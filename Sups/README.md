# <img src="https://github.com/kastaniotis/sups/blob/master/ups.png" style="width:36px;" valign="middle">sups

Sups (for simple ups) is a simple application to do quick queries on usb connected UPS devices that support the HID protocols.

So far it is only tested with Powerwalker Basic UPSs, and works only on linux

## Installation

There are a few options

### Manual

You can find the executable binary in the last release of the github repo. 

https://github.com/kastaniotis/sups/releases

Just untar it and move it to the /usr/local/bin folder so that it is in your path

``` bash
tar -xvzf ./sups.tar.gz
sudo mv ./sups /usr/local/bin
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
sudo sups
```

The response takes a couple of seconds, since the info is broadcasted from the device to the HID interface at random intervals (we are not doing any IOCTL stuff). 

The application tries to detect any HID compatible devices, and tries to connect to the last one it finds.

If you want to define your own dev files, you can do it with the --port argument.

``` 
sudo sups --port /dev/usb/hiddev1
```
<img width="244" alt="image" src="https://github.com/kastaniotis/sups/assets/1822122/88838d11-0be9-4794-8a43-a5401a806fff">

If you want the output in json, you can use the --json argument

Or if you have cloned the repo, you can use "dotnet run" to execute the app in debug mode

``` bash
sudo sups --json
```
<img width="609" alt="image" src="https://github.com/kastaniotis/sups/assets/1822122/60d14614-b5c6-475b-b9f7-d1c915796b0f">

The json data include the following fields

``` json
{
  "Port": "/dev/usb/hiddev0",
  "Charge": 100,
  "ACPresent": true,
  "Time": 32,
  "ChargerStatus": "Charged"
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

- Add support for shutting down the local host at a predefined capacity. 50% is the minimum safe level for Lead Acid batteries, but in my experience most cheap batteries plummet fast. So I will put the default level at something around 30% to give users some reaction time.
- Add support for shutting down remote hosts. Linux or Windows.
- The checks will run via cron and not systemd to minimize dependencies and configuration

## Why C# and not C or C++ or whatever other fancy low level language?

There is no need for ultra-fast code. We are not writting a fast performing app to serve millions of clients. The performance (in this implementation) is swamped by the slow update rate of the hardware anyway.

So we can benefit from a language that makes the "business" side easier to follow.

The drawback is that there is no easy way to call native low level APIs etc. 

I also try not to use idiomatic language structures that make the code difficult to follow. And try to put any frameworks or libraries behind Services that abstract the functionality, making moving to different libraries and frameworks easier.

The application is written using a simple Event Dispatcher. This fits the way the hardware transmits the data in a random way. And makes it easy to plug in new functionality if/when it is required.

## Acknowledgements
Icons are courtesy of the [Noun Project](https://thenounproject.com/)
