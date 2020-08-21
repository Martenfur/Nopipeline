# Nopipeline

![Nopipeline](/pics/Nopipeline.png)

For some reason, the only Monogame's resource manager, Pipeline tool, is a total pain in the ass to work with. A lot of stuff which should've been automated is instead painfully done by hand in a crappy UI. After you've dragged and dropped 100 pngs one by one, you come to realization that the Pipeline tool is nothing but tears and sadness.

## So what shall we do about all this?

Fear not - Nopipeline comes to the rescue. It's an addon for Pipeline Tool, which generates and updates `.mgcb` config for you. You can safely add, delete and move around resource files directly in Explorer - Nopipeline will do the rest for you. 

Additionally, you can make resource files watch other files! Let's say, you got Tiled map project. It has one main `.tmx` file and a bunch of textures and tileset files. But Pipeline Tool has referenced only `.tmx` file, so if you
update only texture or only tileset, you have to either update the `.tmx` or do a manual rebuild, because Pipeline Tool doesn't know about files other than `.tmx`. 


With Nopipeline you don't have to do any of that - just set `.tmx` file to watch textures and tilesets - and Pipeline Tool will detect and update everything by itself.


## Sounds cool and all, but is it compatible with my existing setup/engine/pipeline extensions?

Nopipeline is not a Pipeline Tool replacement - it's an addon. Its only function is to scoop up all resource files and put them into `.mgcb` config. The rest of the resource compilaton process will go exactly the same. You can add or remove Nopipeline at any point in development, and nothing will break. Nopipeline won't override resources which already exist in the `.mgcb` config and will leave a perfectly valid config after itself.


## Now we're talking. How do I integrate this thing in my project?

First of all, install the `Nopipeline` via Nuget. After that, you will need a NPL config. NPL config is what Nopipeline uses to generate MGCB config. It looks like this:


```json
{
	"content": 
	{
		"textures": 
		{
			"path": "Textures/*.png",
			"recursive": "True",
			"action": "build",
			"importer": "TextureImporter",
			"processor": "TextureProcessor",
			"processorParam": 
			{
				"ColorKeyColor": "255,0,255,255",
				"ColorKeyEnabled": "True",
				"GenerateMipmaps": "False",
				"PremultiplyAlpha": "True",
				"ResizeToPowerOfTwo": "False",
				"MakeSquare": "False",
				"TextureFormat": "Color",
			}
		},
		"specificFile": 
		{
			"path": "Path/To/File/specificFile.txt",
			"recursive": "False",
			"action": "copy",
		}
	}
}
```
NPL config is essentially a JSON. Config above has two file groups: `textures` 
and `specificFile`. Each file group describes one specific resource type. 
File groups can contain whole directories or single files.


Let's look at an each parameter:
- `path` is a path to the resource files relative to the main Content folder. 
Here are some examples:
	- `Graphics/Textures/texture.png` will grab only `texture.png` file.
	- `Graphics/Textures/*.png` will grab any `.png` file.
	- `Graphics/Textures/*` will grab any file in the `Textures` directory.
- `recursive` tells Nopipeline to include resource files from subdirectories.
For example, if set to `True`, and the `path` is `Graphics/Textures/*.png`,
files from `Graphics/Textures/Subdir/` will be grabbed as well. If set to 
`False`, they will be ignored.
- `action` tells what action has to be done for this file group. Can be `build`
or `copy`.
- `importer` tells what importer should be used for building.
- `processor` tells what processor should be used for building.
- `processorParam` is an optional list of processor parameters, if resource 
has any.


There is also an optional `watch` parameter. Its usage looks like this:

```json
{
	"content": 
	{
		"spriteGroup": 
		{
			"path": "Graphics/*.spritegroup",
			"recursive": "True",
			"action": "build",
			"importer": "SpriteGroupImporter",
			"processor": "SpriteGroupProcessor",
			"watch": 
			[
				"Default/*.png",
				"Default/*.json",
			]
		},
	}
}
```
With `watch` parameter present, all the `.spritegoup` files will be built by Pipeline Tool, if any `.png` or `.json` file will be changed. Note that all the paths listed in `watch` are relative to the main `path`, so final paths  will look like this: `Graphics/Default/*.png`.

But that's not all. Nopipeline also provides an extended reference management. Add `references` section into your `.npl` config like this:

```json
{
	"references":
	[
		"%PROGRAMFILES%/YourLibDir/Library.dll",
		"RelativePath/RelativeLibrary.dll",
	],
	"content": 
	{

	}
}
```
With Nopipeline you can use environment variables like `%PROGRAMFILES%` - something Pipeline Tool can't do by itself. If referenced libraries are missing, Nopipeline will delete their entries from config. Additionally, you can add references the old way from the Pipeline Tool - Nopipeline will not delete them unless the files themselves don't exist.


With NPL config done, save it in the same directory as MGCB config and give it the same name. For example, if your MGCB config is named `Content/Content.mgcb`, your NPL config should be `Content/Content.npl`

You can also include NPL in Visual Studio project, if you want.

If you want more seamless pipeline-forgetting experience, you can check out [Monofoxe Engine](https://bitbucket.org/Martenfur/monofoxe/src), with Nopipeline integrated out of the box.

## All the other stuffs. 

The thing is licensed under MPL 2.0, so you can use it and its code in anything you want for free.


Huge thanks to [MirrorOfSun](https://github.com/MirrorOfSUn), who wrote most of the code.


*Don't forget to pet your foxes.*
