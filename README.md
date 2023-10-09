# QuantumTunnel
A Xbox One/Series Flash Dumper for SystemOS created in C#.

### Prerequisites
.NET 7 is required, for instructions on installing .NET to your console over SSH follow https://xosft.dev/wiki/installing-compatible-software/.

You must also have an administrator account, with a shell **elevated to SYSTEM** on your console in either Retail or Dev Mode to access the flash driver. 

### Usage
If dotnet isn't in your PATH:
`cd dotnet`
`dotnet QuantumTunnel.dll FileToDump`

If dotnet is in your PATH:
`dotnet QuantumTunnel.dll FileToDump`

f.e. `dotnet QuantumTunnel.dll certkeys.bin`

To obtain a raw flash image, usable in XBFS tools, use --rawdump (1GBish on Series S/X):
`dotnet QuantumTunnel.dll --rawdump -o dump.bin`

If using the published/self-contained build, use `QuantumTunnel.exe` instead of `dotnet QuantumTunnel.dll`

### FAQ
Q: Why can't I dump X file? (host.xvd, system.xvd, etc)
A: Files currently mounted or otherwise in use can't be read.

Q: How do I dump my developer certificate?
A: Read certkeys.bin

Q: Where can I find a list of file names?
A: Check out XVDTool's [source code](https://github.com/emoose/xvdtool/blob/master/LibXboxOne/NAND/XbfsFile.cs#L13)

Q: I get an access denied error
A: Ensure your shell is elevated to **SYSTEM privileges**.
