#include "util.h"

void ReplaceSingleChar(TCHAR * Str, TCHAR From, TCHAR To)
{
	while(*Str++)
		if(*Str == From)
			*Str = To;
}

void ReplaceProtectedString(char * Dst, char * Src)
{
	DWORD mProt;
	size_t strSize;

	if(Dst == NULL || Src == NULL)
		return;

	strSize = strlen(Dst);

	VirtualProtect(Dst, strSize, PAGE_READWRITE, &mProt);
	strcpy_s(Dst, strSize, Src); 
	VirtualProtect(Dst, strSize, mProt, &mProt);
}

BOOL CompareData(PBYTE pData, PBYTE bMask, const char * pszMask)
{
	for(;*pszMask; ++pszMask, ++pData, ++bMask)
		if(*pszMask == 'x' && *pData !=* bMask) 
			return FALSE;
	return (*pszMask) == 0;
}
 
DWORD FindPattern(DWORD dwAddress, DWORD dwLen, PBYTE bMask, const char * pszMask)
{
    register DWORD i;

	for(i = 0; i < dwLen; i++)
		if(CompareData((BYTE*)( dwAddress + i ), bMask, pszMask))
			return (DWORD)(dwAddress + i);

	return 0;
}
