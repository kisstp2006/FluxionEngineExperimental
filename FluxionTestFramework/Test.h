#pragma once

class test
{
public:
	virtual ~test() = default;

	virtual bool init() = 0;
	virtual bool run() = 0;
	virtual bool stop() = 0;
};
