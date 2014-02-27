#pragma once

#include <tchar.h>
#include <Windows.h>

#ifdef __cplusplus
extern "C"
{
#endif

void ReplaceSingleChar(TCHAR * Str, TCHAR From, TCHAR To);
void ReplaceProtectedString(char * Dst, char * Src);
BOOL CompareData(PBYTE pData, PBYTE bMask, const char * pszMask);
DWORD FindPattern(DWORD dwAddress, DWORD dwLen, PBYTE bMask, const char * pszMask);

#ifdef __cplusplus
}
#endif
