#include <Windows.h>
#include <tlhelp32.h>
#include <Psapi.h>
#include <tchar.h>
#include <intrin.h>

#pragma comment(lib, "Psapi.lib")
#pragma comment(lib, "detours.lib")

#define INFO_PATH _T("%TEMP%")
#define INFO_FILE _T("4b01065a-6d66-45aa-a488-c97d314272e9.dat")
#define UPDATE_TIMEOUT 5000 
