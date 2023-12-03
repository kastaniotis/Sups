# <img src="https://raw.githubusercontent.com/kastaniotis/Sups/master/Sups/ups.png" style="width:36px;" valign="middle">sups

sups (for simple ups) is a small application that allows you to do quick queries on usb connected UPS devices with 0 (or minimal) configuration.

![image](https://github.com/kastaniotis/Sups/assets/1822122/d1fa5cc5-8d71-4c1c-851a-eb9369be1087)

The application looks on its own for HID compatible devices under /dev and tries to connect to the first one that it finds.

You can define your own device using the --port argument

```bash
sudo ./sups --port /dev/usb/hiddev1
```

If you want to shutdown the local machine below a certain battery level, you can use the --local-monitoring argument. The default threshold is 50% as this is the minimum safe level for cheap lead acid batteries. But you can also define your own threshold using the argument --threshold and passing the percentage. Like

```bash
sudo sups --local-monitoring --threshold 30
```

It is not meant as a complete replacement for APCUPSD or NUT (or the vendor provided software)

sups focuses on being flexible and easy to use for simple or complex everyday UPS scenarios.

APCUPSD or NUT work like a charm for low level things like battery calibration of specific UPS devices, but are difficult to work with when you need to define complex everyday scenarios, like reading multiple UPSs and shutting down multiple remote devices on separate predefined battery levels in case of a power outage.

sups focuses and excels exactly on those scenarios. It is designed to be simple and flexible enough to allow you to define complex use cases by just calling the app multiple times with different arguments.

For detailed instructions, please check the WIKI

https://github.com/kastaniotis/Sups/wiki/Home
