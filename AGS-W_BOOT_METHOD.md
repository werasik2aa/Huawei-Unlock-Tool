# AGS-W: booting custom firmware

The `AGS-WXX.zip` pack was tagged `FB_ONLY_BROKEN`. It is not broken. After you
flash the pack, the device has a working fastboot and boots custom images, the
same `FB_ONLY` level as AGS-L. The files in `AGS-WXX.zip` and `AGS-LXX.zip` are
identical.

This is not a full unlock. It only enables orange-state (unverified) boot, which
is the normal behaviour for an owner booting their own firmware.

Tested on AGS-W09 (agassi, MSM8917). Mainline postmarketOS boots: 4 CPUs, 2 GB
RAM, eMMC with all 62 partitions, DSI panel, USB gadget. dmesg at the end.

## Enter fastboot

Hold Volume-Down + Power while plugging in USB. The device shows up as
`18d1:d00d`. `fastboot boot <image>` runs an image from RAM. No `oem unlock` is
needed. The engineering aboot skips verification and boots in orange state.

To keep an image, flash it to the `boot` partition. `fastboot boot` is RAM only.

## Why a plain boot.img is rejected

The engineering aboot still reads a signature at `image + image_actual`:

- If the first byte is not `0x30` (DER SEQUENCE), it panics to EDL (9008).
- If the signature is present but does not match, it is accepted. The device
  goes orange and keeps booting.

So the fix is to give it a real, well-formed signature that it will fail to
verify.

## The signed-image method

Use the genuine signature from your own stock `boot` partition. Append it to your
custom image.

1. `image_actual` (page = 2048 bytes):

   ```
   image_actual = page
                + roundup(kernel_size,  page)
                + roundup(ramdisk_size, page)
                + roundup(second_size,  page)
                + roundup(dt_size,      page)
   ```

2. Build the image. Header load addresses are ignored. The aboot forces:

   | part          | address      |
   | ------------- | ------------ |
   | arm64 kernel  | `0x80080000` |
   | ramdisk       | `0x83600000` |
   | tags / DTB    | `0x83400000` |

3. The kernel slot is `Image.gz` followed by one appended DTB (plain libfdt
   walk, no QCDT). The DTB must carry the board tags the aboot selects on this
   unit:

   ```
   qcom,msm-id   = <0x12f 0 0x134 0 0x135 0>
   qcom,board-id = <0x1000b 0>
   ```

4. Read the signature from your stock `boot.img` at its own `image_actual` (1320
   bytes, starts with `0x30`). Append it to your custom image at the custom
   image's `image_actual`, then pad the signature area to 2048 bytes.

5. `fastboot boot <image>`. The aboot parses the signature, the RSA check fails
   because the kernel differs, the device goes orange, and it boots.

A stock Huawei image is already signed and verifies green, so it does not need
this step.

## Example

```sh
# 1. kernel slot = gzip kernel + one tagged DTB
fdtput -t x agassi.dtb / qcom,msm-id   0x12f 0 0x134 0 0x135 0
fdtput -t x agassi.dtb / qcom,board-id 0x1000b 0
cat Image.gz agassi.dtb > kernel

# 2. pack (header addresses are cosmetic; the aboot forces its own)
mkbootimg --kernel kernel --ramdisk initramfs \
  --base 0x80000000 --kernel_offset 0x00080000 \
  --ramdisk_offset 0x03600000 --tags_offset 0x03400000 \
  --pagesize 2048 -o out.unsigned

# 3. append your stock /boot signature at image_actual, pad to 2048
python3 append_sig.py out.unsigned stock_boot.img out.img

# 4. boot
fastboot boot out.img
```

`append_sig.py`: read the 9 size fields at offset 8 of `out.unsigned`, compute
`image_actual`, trim the file to it, then append the 1320-byte signature taken
from `stock_boot.img` at its own `image_actual`, padded to 2048.

## Notes

- The signature is tied to your device. It is yours to use and is useless to
  anyone else, so there is no need to share the bytes.
- Keep the engineering chain in place. It is what enables this.

## Proof: mainline postmarketOS dmesg

```
Booting Linux on physical CPU 0x0000000100 [0x410fd034]
Linux version 7.1.3-msm89x7 ... #2-postmarketOS SMP PREEMPT
Machine model: Huawei Mediapad T3 10 (agassi)
smp: Brought up 1 node, 4 CPUs
SMP: Total of 4 processors activated.
Memory: 1946232K/2097152K available
mmc0: new HS400 MMC card at address 0001
mmcblk0: mmc0:0001 HAG4a2 14.7 GiB
 mmcblk0: p1 p2 p3 ... p62
Setting up USB gadget through configfs
Entering debug shell
```
