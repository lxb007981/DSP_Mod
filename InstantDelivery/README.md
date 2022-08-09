# InstantDelivery 

Mod for Dyson Sphere Program. BepInEx required.

Instantly delivery items from `Remote Supply` interstellar stations to `Remote Demand` interstellar stations.

Basically the stations work as usual (find matching pairs, decide the amount to be sent), however now it requires no vessels and the cargo is delivered instantly. You can think of it as a station equipped with a vessel with infinite speed. Note it still consumes power and warpers, so it won't deliver cargos unless the related research has been unlocked, and the power/warper supply is satisfied. The station is also constrolled by options like Min Load of Vessels, and Transport Range of Vessels. Just as in the original game.

- No vessels required
- Supports Orbital Collectors
- Consumes power and warpers

## Usage

Put `InstantDelivery.dll` into `...\Dyson Sphere Program\BepInEx\plugins`.

## Configurations

There is an option `EnableVessels` to control whether the game continues to send out vessels besides the functionality provided by `InstantDelivery`, and the option also controls whether the game continues to calculate paths for those vessels that have already departed. 

You can find this option after running the game once with the mod installed, and check `...\Dyson Sphere Program\BepInEx\config\lxb007981.dsp.InstantDelivery.cfg`.

If you are using this mod in a new run with no exsiting vessels, you can set the option to `false`. Otherwise if the mod is used against an exsiting save, it might be better to leave the option `true` (with a little bit performance decrease). Since setting it to `false` will freeze all vessels that have departed, and they will never reach their destinations. Or you could manually take out all the vessels from all of the stations and then set the option to `false`.


------------------------------------------------------------------------
# 立刻送达 

需要BepInEx.

瞬间将货物从`星际供应`物流站传送到`星际需求`站。

Mod不会改变星际物流站的工作方式。尽管无需运输船，但星际物流站仍会消耗电力和翘曲器，而且受到运输船起送量、最远运送距离等选项控制。你可以将此Mod理解为将星际物流站变为不需要运输船也能工作，且运送速度无限。

- 无需运输船
- 支持轨道采集器
- 仍需消耗电力与翘曲器

## 使用方法

将`InstantDelivery.dll`放入`...\Dyson Sphere Program\BepInEx\plugins`.

## 配置

`EnableVessels`选项用于控制物流站是否仍如未安装Mod时一样发送运输船。将此选项置为`false`会同时冻结已经出发的运输船（因为有关发送运输船、校正轨道的代码不再被运行）。

在安装Mod并运行一次游戏后，在`...\Dyson Sphere Program\BepInEx\config\lxb007981.dsp.InstantDelivery.cfg`找到配置文件。

如果此Mod被用在新存档（没有运输船），可以将选项置否（获得一点性能提升）。

如果此Mod被用在已有运输船运行的旧存档，建议不改动默认选项（true）。
你也可以手动取走所有物流站里的运输船后再改动选项。

## Release Notes

### v0.1.0

- Initial Release

