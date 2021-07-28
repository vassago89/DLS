#pragma once

#include <string>
#include <iostream>

using namespace System;
using namespace Runtime::InteropServices;

static void MarshalWString(String ^ s, std::wstring* wstr) {
	auto chars = (const wchar_t*)(Marshal::StringToHGlobalUni(s)).ToPointer();
	*wstr = (chars);
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}

static void MarshalString(String^ s, std::string* str) {
	auto chars = (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
	*str = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}