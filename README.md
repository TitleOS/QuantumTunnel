# QuantumTunnel
A Xbox One/Series Flash Dumper for SystemOS created in C#.

### Prerequisites
.NET 6 is required, for instructions on installing .NET to your console over SSH follow https://xosft.dev/wiki/installing-compatible-software/.

You must also have an administrator account, with a shell elevated to SYSTEM on your console in either Retail or Dev Mode to access the flash driver. 

### Usage
If dotnet isn't in your PATH:
`cd dotnet`
`dotnet QuantumTunnel.dll FileToDump`

If dotnet is in your PATH:
`QuantumTunnel.exe FileToDump`

To obtain a raw flash image, usable in XBFS tools, use -rawdump (1GBish on Series S/X):
`dotnet QuantumTunnel.dll -rawdump`

### FAQ
Q: Why can't I dump X file? (host.xvd, system.xvd, etc)
A: Files currently mounted or otherwise in use can't be read.

Q: How do I dump my developer certificate?
A: Read certkeys.bin

Q: Where can I find a list of file names?
A: Check out XVDTool's [source code](https://github.com/emoose/xvdtool/blob/master/LibXboxOne/NAND/XbfsFile.cs#L13)

