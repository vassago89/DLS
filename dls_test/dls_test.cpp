#include "pch.h"
#include "CppUnitTest.h"
#include "../dls_unmanaged/framework.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace dlstest
{
	TEST_CLASS(dlstest)
	{
	public:
		
		TEST_METHOD(TestMethod1)
		{
			std::vector<unsigned char*> vector = CaptureVideo("test.mp4");
		}
	};
}
