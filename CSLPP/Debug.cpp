#include <windows.h>
#include <iostream>

#define export __declspec(dllexport)

HANDLE hConsole;
HWND hWnd;

extern "C"
{
	export int CreateConsole(LPCWSTR lpConsoleTitle)
	{
		if (AllocConsole())
		{
			hWnd = GetConsoleWindow();

			FILE *pBuffer;
			freopen_s(&pBuffer, "CONOUT$", "w", stdout);
			freopen_s(&pBuffer, "CONOUT$", "w", stderr);
			freopen_s(&pBuffer, "CONIN$", "r", stdin);

			if (!SetConsoleTitle(lpConsoleTitle)) return NULL;

			hConsole = CreateFile(L"CONOUT$",
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		}

		return GetLastError();
	}

	export int DestroyConsole()
	{
		PostMessage(hWnd, WM_QUIT, 0, 0);
		return GetLastError();
	}

	export int SetTextAttr(int nTextAttr)
	{
		SetConsoleTextAttribute(hConsole, nTextAttr);
		return GetLastError();
	}

	export int SetMode(DWORD nConsoleMode)
	{
		SetConsoleMode(hConsole, nConsoleMode);
		return GetLastError();
	}

	export int GetMode(LPDWORD nConsoleMode)
	{
		GetConsoleMode(hConsole, nConsoleMode);
		return GetLastError();
	}

	export int Write(LPCSTR lpOutputText)
	{
		std::cout << lpOutputText;
		return GetLastError();
	}

	export int SetIcon(HICON hIcon)
	{
		SendMessage(hWnd, WM_SETICON, ICON_BIG, reinterpret_cast<LPARAM>(hIcon));
		if (GetLastError() != 0) return GetLastError();

		SendMessage(hWnd, WM_SETICON, ICON_SMALL, reinterpret_cast<LPARAM>(hIcon));
		return GetLastError();
	}

	export int SetIconBig(HICON hIcon)
	{
		SendMessage(hWnd, WM_SETICON, ICON_BIG, reinterpret_cast<LPARAM>(hIcon));
		return GetLastError();
	}

	export int SetIconSmall(HICON hIcon)
	{
		SendMessage(hWnd, WM_SETICON, ICON_SMALL, reinterpret_cast<LPARAM>(hIcon));
		return GetLastError();
	}
}