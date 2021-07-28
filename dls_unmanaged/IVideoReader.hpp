#pragma once
namespace dlsunmanaged {
	struct VideoInfo
	{
		int FrameWidth;
		int FrameHeight;
		int Channels;

		int FrameCount;
		int FramePos;

		double FPS;
	};

	__interface IVideoReader
	{
	public:
		bool Read(unsigned char*& buffer);
		void Jump(int index);
		void Release();

		bool IsDone();
		int GetCount();
		int GetPos();
		int GetWidth();
		int GetHeight();
		int GetChannels();
		void SetPos(int pos);
	};
}