---
name: UPS model support request
about: Use this template to request support for your UPS
title: ''
labels: ''
assignees: ''

---

sups works with USB connected UPS devices that support the HID protocol.

**Device Details**
Please provide full details about your UPS model, like make, model, year, connectivity etc

**Initial Compatibility check**
Please read the instructions in the Wiki page https://github.com/kastaniotis/Sups/wiki/5.-Supported-UPS-Devices-and-HID and provide below a complete output of the hiddev device.

For example

```bash
sudo cat /dev/usb/hiddev0 | od -tx1 -Anone
```
And the output should be something like

```
66 00 85 00 50 00 00 00 68 00 85 00 dc 05 00 00
2a 00 85 00 2c 01 00 00 d0 00 85 00 01 00 00 00
44 00 85 00 01 00 00 00 45 00 85 00 00 00 00 00
42 00 85 00 00 00 00 00 46 00 85 00 00 00 00 00
43 00 85 00 00 00 00 00 66 00 85 00 50 00 00 00
```

**Non-HID UPS Devices**
I can investigate how specific devices could work with sups, but I it will require significant effort
