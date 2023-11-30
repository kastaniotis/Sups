# <img src="https://raw.githubusercontent.com/kastaniotis/Sups/master/Sups/ups.png" style="width:36px;" valign="middle">sups

sups (for simple ups) is a simple application to do quick queries on usb connected UPS devices that support the HID protocols.

It is not meant as a replacement for APCUPSD or NUT. 

It is meant as an easy to maintain and use alternative that provides the most common functionality required by a UPS.

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

Please note that you will also need clang installed, since the project is configured for Ahead of Time compilation.

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
![image](https://github.com/kastaniotis/sups/assets/1822122/06eb1e6f-92e3-4ff5-803d-78c194633e14)

If you want the output in json, you can use the --json argument

Or if you have cloned the repo, you can use "dotnet run" to execute the app in debug mode

``` bash
sudo sups --json
```
![image](https://github.com/kastaniotis/sups/assets/1822122/53c72a82-0082-4457-aaab-11503a059a7e)

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

## Battery Level monitoring

You can use the --local-monitoring argument to enable monitoring of the battery level. The application will shutdown the local machine if the level is below
the predefined threshold. The default is 50% which is the minimum safe level for Lead Acid batteries.

If you want to define your own threshold, you can use the argument --threshold and pass it the new percentage

``` bash
sudo sups --local-monitoring --threshold 30
```

The application cannot run as a service, so if you want to automatically shut the machine down, you have to run the app through cron.

To run it every minute, you should edit root's cron file with

``` bash
sudo crontab -e
```
And add something like the following at the end of the file

``` bash
* * * * * sups --local-monitoring --threshold 45
```

The app shuts the machine immediately sending the command below to the system

``` bash
shutdown -P now
```
No delay is defined, since the minimum is 1 minute and this might create race conditions with cron running the 
command again, resetting the shutdown timer.

## Controlling Remote Devices

The application can shutdown remote linux devices using the argument --remote-monitoring and passing it the IP or FQDN of 
the target machine.

```bash
sudo sups --remote-monitoring 192.168.2.100 --threshold 50
```

This option gives you great freedom and flexibility. With multiple cron entries, you can control multiple UPS devices 
and target machines, with different thresholds.

**YOU SHOULD NOT MIX LOCAL AND REMOTE MONITORING ON THE SAME CALL**

Ideally, you want separate calls for each device, keeping in mind that your controller machine (local) has to shutdown
last, so it should have the lowest threshold of all devices.

For example

```bash
sudo sups --port /dev/usb/hiddev0 --remote-monitoring device1.domain.com --threshold 90
sudo sups --port /dev/usb/hiddev1 --remote-monitoring device2.domain2.com --threshold 50
sudo sups --port /dev/usb/hiddev2 --remote-monitoring 192.168.2.142 --threshold 60
sudo sups --port /dev/usb/hiddev3 --local-monitoring --threshold 30
```



The application assumes the following things, which we have to configure

- SSH connection to port 22. For now this cannot be changed. 
Changing ssh port is a very fragile and thin security layer anyway. 
It's far better to use port 22 and treat it as a honeypot using fail2ban or crowdsec.
- SSH connection using the user "ups" using keys
- The key is at /home/ups/.ssh/id_rsa
- The user "ups" is part of the sudoers and can execute shutdown without having to type a password

### Remote Control Configuration Required

You should create the user on all machines that are expected to participate in UPS functionality

```bash
sudo adduser ups
sudo usermod -aG sudo ups 
```

Then, on your controller machine, you have to create the keys for this user and push it to all target machines.

```bash
# Switch to ups user
sudo su - ups

# Create keys for the user
# Follow the instructions and save the private key at /home/ups/id_rsa. You might have to fix permissions.
ssh-keygen -t rsa

# Push the public key to all the devices/ips where it is going to be used
ssh-copy-id -i /home/ups/.ssh/id_rsa.pub ups@192.168.2.105
```

And finally, on all target machines, we have to allow the user "ups" to call shutdown without having to type a password

```bash
# Use visudo to edit sudo permissions
sudo visudo

# At the end of the file add the following
ups ALL = NOPASSWD: /usr/sbin/shutdown

```

The command used internally by the application is the following

```bash
sudo ssh ups@192.168.2.100 -i /home/ups/.ssh/id_rsa -t "sudo /usr/sbin/shutdown -P now"
```

### Security concerns

This guide just provides a baseline that allows a functional sups application.
It is not meant as a rock solid guide on how to secure your users or your setup. 
Please exercise common sense and follow a good guide on how to harden your network, users, their permissions 
and the services that you provide.


## Supported UPS Devices and HID 

The app uses the HID data transmitted by the UPS. 
So far it is only tested with Powerwalker Basic UPSs, and works only on linux

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

## Acknowledgements
Icons are courtesy of the [Noun Project](https://thenounproject.com/)
