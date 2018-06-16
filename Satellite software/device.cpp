#include "stdafx.h"
#include "device.h"

RODevice::RODevice()
{
}

RODevice::RODevice(unsigned short pinIN) : pIN(pinIN)
{
}

RODevice::~RODevice()
{
}

float RODevice::read()
{
	/* This part is undone and need finishing */
	return 0.0f;
}

WODevice::WODevice()
{
}

WODevice::WODevice(unsigned short pinOUT) : pOUT(pinOUT)
{
}

void WODevice::write(const float)
{
	/* This part is undone and need finishing */
}

RWDevice::RWDevice()
{
}

RWDevice::RWDevice(unsigned short pinIN, unsigned short pinOUT) : dIn(pinIN),dOut(pinOUT)
{
}

void RWDevice::write(const float c)
{
	dOut.write(c);
}

float RWDevice::read()
{
	return dIn.read();
}

MISC::MISC()
{
}

MISC::MISC(unsigned short pin) : misc(pin)
{
}

MISC::~MISC()
{
}

Vector<float> MISC::GetGases(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return Vector<float>();
}

Clock::Clock()
{
}

Clock::Clock(unsigned short pin) : clock(pin)
{
}

Clock::~Clock()
{
}

float Clock::GetTime(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

BMP::BMP()
{
}

BMP::BMP(unsigned short pin) : bmp(pin)
{
}

BMP::~BMP()
{
}

float BMP::GetPressure(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

float BMP::GetHeight(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

DHT::DHT()
{
}

DHT::DHT(unsigned short pin) : dht(pin)
{
}

DHT::~DHT()
{
}

float DHT::GetTemperature(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

float DHT::GetHumidity(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

float VoltageDivider::calculateVoltage(const float voltFirst, const float voltSecond)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

VoltageDivider::VoltageDivider(){
}

VoltageDivider::VoltageDivider(unsigned short pin1, unsigned short pin2) : voltageFirst(pin1),voltageSecond(pin2)
{
}

VoltageDivider::~VoltageDivider()
{
}

float VoltageDivider::GetVoltage(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

PitoTube::PitoTube()
{
}

PitoTube::PitoTube(unsigned short pin) : pt(pin)
{
}

PitoTube::~PitoTube()
{
}

float PitoTube::GetSpeed(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

LIS::LIS()
{
}

LIS::LIS(unsigned short pin) : lis(pin)
{
}

LIS::~LIS()
{
}

float LIS::GetMagField(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return 0.0f;
}

Servo::Servo()
{
}

Servo::Servo(unsigned short pin) : servo(pin)
{
}

Servo::~Servo()
{
}

void Servo::TurnDegree(const float)
{
	/* This part is undone and need finishing */
}

void Servo::TurnRadians(const float)
{
	/* This part is undone and need finishing */
}

Beeper::Beeper()
{
}

Beeper::Beeper(unsigned short pin) : beeper(pin)
{
}

Beeper::~Beeper()
{
}

void Beeper::Start()
{
	/* This part is undone and need finishing */
}

void Beeper::Stop()
{
	/* This part is undone and need finishing */
}

GPS::GPS()
{
}

GPS::GPS(unsigned short pIN, unsigned short pOUT) : gps(pIN,pOUT)
{
}

GPS::~GPS()
{
}

Vector<float> GPS::GetCoordinates(unsigned int timeout)
{
	/* This part is undone and need finishing */
	return Vector<float>();
}

Camera::Camera()
{
}

Camera::Camera(unsigned short pIN, unsigned short pOUT) : camera(pIN,pOUT)
{
}

Camera::~Camera()
{
}

void Camera::TakePhoto()
{
	/* This part is undone and need finishing */
}

XBee::XBee()
{
}

XBee::XBee(unsigned short pIN, unsigned short pOUT) : xbee(pIN,pOUT)
{
}

XBee::~XBee()
{
}

String XBee::read()
{
	/* This part is undone and need finishing */
	return nullptr;
}

void XBee::send(const String msg)
{
	/* This part is undone and need finishing */
}

void XBee::send(const Vector<byte> data)
{
	/* This part is undone and need finishing */
}
