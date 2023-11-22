# <img src="https://github.com/kastaniotis/Sups/blob/master/Sups/ups.png" style="width:36px;" valign="middle">sups

Sups (for simple ups) is a simple application to do quick queries on usb connected UPS devices that support the HID protocols.

It is not meant as a replacement for APCUPSD or NUT. 

It is meant as an easy to maintain and use alternative that provides the most common functionality required by a UPS.

So far it is only tested with Powerwalker Basic UPSs, and works only on linux

## Installation

There are a few options

### Manual

You can find the executable binary in the last release of the github repo. 

https://github.com/kastaniotis/Sups/releases

Just untar it and move it to the /usr/local/bin folder so that it is in your path

``` bash
tar -xvzf ./Sups.tar.gz
sudo mv ./Sups /usr/local/bin
```

### Compilation

You can clone the repo and compile it with dotnet since the app is written in C#. 

There is a script that does it for you and copies the binary in your path

``` bash
./publish.sh
```

## Running the Application

You might need root permissions to run the application, depending on the ownership of the dev file. 

``` bash
sudo Sups
```

The response takes a couple of seconds, since the info is broadcasted from the device to the HID interface at random intervals (we are not doing any IOCTL stuff). 

The application tries to detect any HID compatible devices, and tries to connect to the last one it finds.

If you want to define your own dev files, you can do it with the --port argument.

``` 
sudo Sups --port /dev/usb/hiddev1
```
![image](https://github.com/kastaniotis/Sups/assets/1822122/06eb1e6f-92e3-4ff5-803d-78c194633e14)

If you want the output in json, you can use the --json argument

Or if you have cloned the repo, you can use "dotnet run" to execute the app in debug mode

``` bash
sudo Sups --json
```
![image](https://github.com/kastaniotis/Sups/assets/1822122/53c72a82-0082-4457-aaab-11503a059a7e)

The json data include the following fields

``` json
{
    "Port": "/dev/usb/hiddev0", 
    "Charge": 100,
    "ACPresent": true,
    "Time": 40,
    "ChargerStatus": "Charged",
    "ShutdownThreshold": 50,
    "Monitoring": false
}
```

You can also use the --debug argument to display extra information about the data received and how they are being parsed.

## Supported UPS Devices and HID 

The app uses the HID data transmitted by the UPS. 

To find the device, you can use

``` bash
find /dev -name hidd*
```

You will get something like

``` bash
/dev/usb/hiddev1
```

If you still do not know which one is your UPS, you can run the command before and after connecting the UPS to usb. 

To do an initial compatibility check you can use 

``` bash
sudo cat /dev/usb/hiddev0 | od -tx1 -Anone
```

It should return something like 

``` bash
 66 00 85 00 50 00 00 00 68 00 85 00 dc 05 00 00
 2a 00 85 00 2c 01 00 00 d0 00 85 00 01 00 00 00
 44 00 85 00 01 00 00 00 45 00 85 00 00 00 00 00
 42 00 85 00 00 00 00 00 46 00 85 00 00 00 00 00
 43 00 85 00 00 00 00 00 66 00 85 00 50 00 00 00
```

The data is in 8 byte sets, and follow the rules found in the pdf below (P. 375)
https://usb.org/sites/default/files/hut1_4.pdf 

The first byte is the "usage", which is basically the property. 

The third byte is expected to be 85 since this is about a battery.

The fifth and sixth bytes are the value.

The app uses the Usages below (first byte):

- 2a: Remaining Time Limit
- 42: Below Remaining Battery Limit
- 43: Remaining Time Limit Expired
- 44: Charging
- 45: Discharging
- 46: Fully charged
- 66: Remaining Capacity
- 68: Run Time To Empty
- d0: AC Present

42, 43 and 2A are ignored for now since we are not doing any fancy decision making
with the UPS's tooling.

44,45 and 46 give a Charger Status control that works like a radio button, giving 
"Charging", "Discharging", or "Charged"

If you have the patience to see how HID is supposed to work with those "usages", 
you can use the pdf as a refference, and then you need your device's "report descriptor" 
that can be found using the command below

``` bash
cat /sys/class/hidraw/hidraw0/device/report_descriptor | od -tx1 -Anone
```

And then you can use the tool below to translate it into a "meaningful" descriptor

https://eleccelerator.com/usbdescreqparser/

## TODO
This is the feature list that I intend to include on v1

- Add support for shutting down remote hosts. Linux or Windows.

## Why C# and not C or C++ or whatever other fancy low level language?

There is no need for ultra-fast code. We are not writting a fast performing app to serve millions of clients. The performance (in this implementation) is swamped by the slow update rate of the hardware anyway.

So we can benefit from a language that makes the "business" side easier to follow.

The drawback is that there is no easy way to call native low level APIs etc. 

I also try not to use idiomatic language structures that make the code difficult to follow. And try to put any frameworks or libraries behind Services that abstract the functionality, making moving to different libraries and frameworks easier.

The application is written using a simple Event Dispatcher. This fits the way the hardware transmits the data in a random way. And makes it easy to plug in new functionality if/when it is required.

## Acknowledgements
Icons are courtesy of the [Noun Project](https://thenounproject.com/)
