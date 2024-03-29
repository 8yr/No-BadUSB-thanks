# No BadUSB thanks
A project to defend against BadUSB attacks like (USB-Raspberry Pi, USB-Rubber Ducky)

## Pictures of BadUSB 
Raspberry Pi
![images](https://github.com/8yr/No-BadUSB-thanks/assets/63230850/f1dd1936-9353-4eb8-b74b-afc43dd541fb)


Rubber Ducky
![61RBVTA-7WL _AC_UF350,350_QL80_](https://github.com/8yr/No-BadUSB-thanks/assets/63230850/4dad38f0-008b-46b6-bc2f-51742c04363c)

# How it works?

Utilizes several techniques, including scanning files with specific extensions such as .dd and .bin and deleting them if possible. It also checks the USB device name to ensure it's not malware. Afterward, it freezes all keyboards and mice connected to the device, followed by a system restart.

## Demo

- Without No BadUSB thanks


https://github.com/8yr/No-BadUSB-thanks/assets/63230850/66bd2267-549b-4c0d-ba4a-7bb9a0f2ecb8



- With No BadUSB thanks


https://github.com/8yr/No-BadUSB-thanks/assets/63230850/310c5074-f2ab-46b9-9f10-4f313e775652



## Installation

Install it from the Releases section and run it with administrator privileges.

## Problems

- It freezes all keyboards and mice connected to the device instead of specifically targeting the BadUSB

- Mandatory system restart.

## Reference

  [What is BadUSB?](https://en.wikipedia.org/wiki/BadUSB)
