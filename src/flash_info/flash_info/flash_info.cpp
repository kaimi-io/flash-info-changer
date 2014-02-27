#include "flash_info.h"
#include "detours.h"
#include "util.h"

#ifdef _DEBUG
#define LOG LogMessage
#else
#define LOG(...)
#endif

typedef struct
{
    char os_name[64];
    char os_lang[64];
    int width;
    int height;
    char flash_ver1[64];
    char flash_ver2[64];
} flash_info;


const unsigned char OS_SIG[] = {0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x83, 0xF8, 0x0E, 0x77, 0x00, 0xFF, 0x24, 0x85, 0x00, 0x00, 0x00, 0x00, 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xB8, 0x00, 0x00, 0x00, 0x00};
const char * OS_MASK = "x????xxxxx?xxx????x????xx????xx????xx????";

const unsigned char LANG_SIG[] = {0x8B, 0x44, 0x24, 0x04, 0x8B, 0xC8, 0x81, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x49, 0x83, 0xF9, 0x00, 0x0F, 0x87, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x24, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x25};
const char * LANG_MASK = "xxxxxxxx????xxx?xx????xxx????x";

const unsigned char VER1_SIG[] = {0xB0, 0x01, 0x56, 0x8B, 0xF1, 0x88, 0x46, 0x04, 0x88, 0x46, 0x05, 0x88, 0x46, 0x06, 0x88, 0x46, 0x07, 0x88, 0x46, 0x08, 0x88, 0x46, 0x09, 0x88, 0x46, 0x0A};
const char * VER1_MASK = "xxxxxxxxxxxxxxxxxxxxxxxxxx";

const unsigned char VER2_SIG[] = {0x83, 0x7C, 0x24, 0x04, 0x08, 0x7F, 0x0C, 0x80, 0x7C, 0x24, 0x08, 0x00, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x74, 0x05, 0xB8, 0x00, 0x00, 0x00, 0x00, 0xC3};
const char * VER2_MASK = "xxxxxxxxxxxxx????xxx????x";

HMODULE hFlashDll = NULL;
DWORD FlashDllSize = 0;
flash_info Info;
HANDLE UpdateThread;

int (WINAPI * RealGetSystemMetrics)(int nIndex) = GetSystemMetrics;
int WINAPI MyGetSystemMetrics(int nIndex);

typedef const char *(__cdecl * ftConstChar)();
ftConstChar RealGetOSVersion;
const char * __cdecl MyGetOSVersion();

typedef const char *(__cdecl * ftConstCharArg)(int);
ftConstCharArg RealGetOSLang;
const char * __cdecl MyGetOSLang(int);

typedef int (__thiscall * ftIntThis)(int, int);
ftIntThis RealGetFlashInfo;
int MyGetFlashInfo();

typedef const char *(__cdecl * ftConstCharArg2)(int, int);
ftConstCharArg2 RealGetFlashInfo2;
const char * __cdecl MyGetFlashInfo2(int, int);

DWORD WINAPI InfoUpdateProc(LPVOID Arg);


void LogMessage(LPCTSTR Format, ...)
{
    TCHAR Buffer[128];
    va_list vArgs;

    va_start(vArgs, Format);

    ZeroMemory(Buffer, sizeof(Buffer));
    _vsntprintf_s(Buffer, _ARRAYSIZE(Buffer), _TRUNCATE, Format, vArgs);

    va_end(vArgs);
    
    OutputDebugString(Buffer);
}

BOOL AttachFlash()
{
    DWORD Aux, FuncAddr = 0;
    HMODULE Modules[1024];
    TCHAR ModName[MAX_PATH];
    IMAGE_DOS_HEADER * dosHeader;
    IMAGE_NT_HEADERS * peHeader;
    unsigned int i;


    LOG(_T("(%d) AttachFlash"), __LINE__);

    /* Find Flash DLL in the current process */
    if(!EnumProcessModules(GetCurrentProcess(), Modules, sizeof(Modules), &Aux))
    {
        LOG(_T("(%d) EnumProcessModules - %08X"), __LINE__, GetLastError());
        return FALSE;
    }

    for(i = 0; i < (Aux / sizeof(HMODULE)); i++)
    {
        if(!GetModuleFileNameEx(GetCurrentProcess(), Modules[i], ModName, ARRAYSIZE(ModName)))
            continue;

        if(_tcsstr(ModName, _T("NPSWF32")) != NULL)
        {
            hFlashDll = Modules[i];
            break;
        }
    }

    LOG(_T("(%d) hFlashDll - %08X"), __LINE__, hFlashDll);

    if(hFlashDll == NULL)
        return FALSE;

    /* Get image size */
    dosHeader = (IMAGE_DOS_HEADER *) hFlashDll;
    peHeader = (IMAGE_NT_HEADERS *)( (DWORD) dosHeader + (DWORD) dosHeader->e_lfanew );

    FlashDllSize = (DWORD) peHeader->OptionalHeader.SizeOfImage;

    LOG(_T("(%d) FlashDllSize - %08X"), __LINE__, FlashDllSize);

    if(FlashDllSize == 0)
        return FALSE;


    /* Find OS Version function address by pattern */
    FuncAddr = FindPattern((DWORD) hFlashDll, FlashDllSize, (PBYTE) OS_SIG, OS_MASK);

    LOG(_T("(%d) OS Version - %08X"), __LINE__, FuncAddr);

    if(FuncAddr == 0)
        return FALSE;

    RealGetOSVersion = (ftConstChar) FuncAddr;


    /* Find OS Lang function */
    FuncAddr = FindPattern((DWORD) hFlashDll, FlashDllSize, (PBYTE) LANG_SIG, LANG_MASK);

    LOG(_T("(%d) OS Lang - %08X"), __LINE__, FuncAddr);

    if(FuncAddr == 0)
        return FALSE;

    RealGetOSLang = (ftConstCharArg) FuncAddr;


    /* Find plugin version function */
    FuncAddr = FindPattern((DWORD) hFlashDll, FlashDllSize, (PBYTE) VER1_SIG, VER1_MASK);

    LOG(_T("(%d) PlugIn Version (Part 1) - %08X"), __LINE__, FuncAddr);

    if(FuncAddr == 0)
        return FALSE;

    RealGetFlashInfo = (ftIntThis) FuncAddr;


    FuncAddr = FindPattern((DWORD) hFlashDll, FlashDllSize, (PBYTE) VER2_SIG, VER2_MASK);

    LOG(_T("(%d) PlugIn Version (Part 2) - %08X"), __LINE__, FuncAddr);

    if(FuncAddr == 0)
        return FALSE;

    RealGetFlashInfo2 = (ftConstCharArg2) FuncAddr;


    ZeroMemory(&Info, sizeof(flash_info));
    
    /* Set thread for info updating */
    UpdateThread = CreateThread(NULL, 0, InfoUpdateProc, NULL, 0, NULL);
    if(UpdateThread == NULL)
    {
        LOG(_T("(%d) CreateThread - %08X"), __LINE__, GetLastError());
        return FALSE;
    }

	/* Detours hooks */
    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
	DetourAttach(&(PVOID&)RealGetSystemMetrics, MyGetSystemMetrics);
    DetourAttach(&(PVOID&)RealGetOSVersion, MyGetOSVersion);
    DetourAttach(&(PVOID&)RealGetOSLang, MyGetOSLang);
    DetourAttach(&(PVOID&)RealGetFlashInfo, MyGetFlashInfo);
    DetourAttach(&(PVOID&)RealGetFlashInfo2, MyGetFlashInfo2);
    DetourTransactionCommit();

	return TRUE;
}

BOOL DetachFlash()
{
    if(UpdateThread)
    {
        TerminateThread(UpdateThread, 0);
        CloseHandle(UpdateThread);
    }

    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourDetach(&(PVOID&)RealGetSystemMetrics, MyGetSystemMetrics);
    DetourDetach(&(PVOID&)RealGetOSVersion, MyGetOSVersion);
    DetourDetach(&(PVOID&)RealGetOSLang, MyGetOSLang);
    DetourDetach(&(PVOID&)RealGetFlashInfo, MyGetFlashInfo);
    DetourDetach(&(PVOID&)RealGetFlashInfo2, MyGetFlashInfo2);
    DetourTransactionCommit();

    return TRUE;
}

BOOL IsBelongsToModule(PVOID Base, PVOID Addr)
{
    MEMORY_BASIC_INFORMATION MBI;
    
    if(!VirtualQuery(Addr, &MBI, sizeof(MEMORY_BASIC_INFORMATION)))
        return FALSE;

    return MBI.AllocationBase == Base ? TRUE : FALSE;
}

const char * __cdecl MyGetOSVersion()
{
    LOG(_T("(%d) GetOSVersion"), __LINE__);

    return Info.os_name;
}

const char * __cdecl MyGetOSLang(int Aux)
{
    LOG(_T("(%d) GetOSLang"), __LINE__);

    return Info.os_lang;
}

__declspec(naked) int MyGetFlashInfo()
{
    static const char * a = "PlugIn", * b = "5.1";
    _asm
    {
        mov     al, 1
        push    esi
        mov     esi, ecx
        mov     [esi+4], al
        mov     [esi+5], al
        mov     [esi+6], al
        mov     [esi+7], al
        mov     [esi+8], al
        mov     [esi+9], al
        mov     [esi+0Ah], al
        mov     [esi+0Bh], al
        mov     [esi+0Ch], al
        xor     al, al
        mov     [esi+0Fh], al

        mov     eax, [a]
        mov     dword ptr [esi+10h], eax
        
        mov     [esi+14h], al
        mov     [esi+15h], al
        mov     [esi+17h], al
        mov     [esi+18h], al

        mov     eax, [b]
        mov     dword ptr [esi+1Ch], eax

        mov     dword ptr [esi+20h], 48h

        lea     eax, Info.flash_ver1
        mov     dword ptr [esi], eax

        mov     [esi+0Dh], al
        mov     [esi+0Eh], al
        mov     [esi+16h], al
        mov     [esi+17h], al
        mov     eax, esi
        pop     esi
        retn    4
    }
}

const char * MyGetFlashInfo2(int, int)
{
    return Info.flash_ver2;
}

int WINAPI MyGetSystemMetrics(int nIndex)
{
    if(IsBelongsToModule(hFlashDll, _ReturnAddress()))
    {
        LOG(_T("(%d) MyGetSystemMetrics"), __LINE__);

        switch(nIndex)
        {
            case SM_CXSCREEN:
                return Info.width;
            break;

            case SM_CYSCREEN:
                return Info.height;
            break;
        }
    }
    
    return RealGetSystemMetrics(nIndex);
}


DWORD WINAPI InfoUpdateProc(LPVOID Arg)
{
    HANDLE File = INVALID_HANDLE_VALUE;
    DWORD FileSize, Aux;
    TCHAR FilePath[MAX_PATH], * Ptr;
    LPVOID Memory = NULL;
    size_t CSize;
    char MsgA[256];
    wchar_t MsgW[256];

    LOG(_T("(%d) InfoUpdateProc"), __LINE__);

    if(!ExpandEnvironmentStrings(INFO_PATH, FilePath, _ARRAYSIZE(FilePath)))
	{
		LOG(_T("(%d) ExpandEnvironmentStrings - %08X"), __LINE__, GetLastError());
		return 1;
	}
    
    /* Truncate redundant temporary path extension (present on Win7) */
    /* Append trailing backslash for other cases */
	Ptr = _tcsstr(FilePath, _T("acro_rd_dir"));
	if(Ptr != NULL)
	{
		*(Ptr) = 0;
		*(Ptr + 1) = 0;
	}
    else
        lstrcat(FilePath, _T("\\"));
    
	lstrcat(FilePath, INFO_FILE);
	LOG(_T("(%d) FilePath - %s"), __LINE__, FilePath);


    while(1)
    {
        File = CreateFile(FilePath, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
        if(File == INVALID_HANDLE_VALUE)
        {
            LOG(_T("(%d) CreateFile - %08X"), __LINE__, GetLastError());
            goto cleanup;
        }

        FileSize = GetFileSize(File, NULL);

        Memory = VirtualAlloc(NULL, FileSize, MEM_COMMIT, PAGE_READWRITE);
        if(Memory == NULL)
        {
            LOG(_T("(%d) VirtualAlloc - %08X"), __LINE__, GetLastError());
            goto cleanup;
        }

        if(!ReadFile(File, Memory, FileSize, &Aux, NULL))
        {
            LOG(_T("(%d) VirtualAlloc - %08X"), __LINE__, GetLastError());
            goto cleanup;
        }

        CopyMemory(&Info, Memory, sizeof(flash_info));

        wsprintfA
        (
            MsgA,
            "(%d) FlasInfo - OSName: %s; OSLang: %s; Width: %d; Height: %d; FlashVer1: %s; FLashVer2: %s",
            __LINE__,
            Info.os_name, Info.os_lang, Info.width, Info.height, Info.flash_ver1, Info.flash_ver2
        );

        mbstowcs_s(&CSize, MsgW, ARRAYSIZE(MsgA), MsgA, _TRUNCATE);
        
        LOG(_T("%ws"), MsgW);


cleanup:

        if(File != INVALID_HANDLE_VALUE)
        {
            CloseHandle(File);
            File = INVALID_HANDLE_VALUE;
        }

        if(Memory)
        {
            VirtualFree(Memory, 0, MEM_RELEASE);
            Memory = NULL;
        }

        Sleep(UPDATE_TIMEOUT);
    }
}

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID reserved)
{
    if(dwReason == DLL_PROCESS_ATTACH)
    {
        LOG(_T("(%d) DLL_PROCESS_ATTACH"), __LINE__);

        AttachFlash();
    }
	else if(dwReason == DLL_PROCESS_DETACH)
	{
        LOG(_T("(%d) DLL_PROCESS_DETACH"), __LINE__);

		DetachFlash();
	}
 
    return TRUE;
}
