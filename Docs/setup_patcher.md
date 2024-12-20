## Patcher setup
Using the native patcher, create a FTP server with anonymous read access, using the following directory structure:
- Maple
  - download
    - FullVersion
      - MSSetup.exe - Used if patching fails for some reason.
  - patch
    - notice
      - 000yy.txt - The version changes file.
    - patchdir
        - 000yy
          - 000xxto000yy.patch - The patch file, generated by the PatchCreator (using `make-patches`)
          - NewPatcher.dat - The new patcher to download (renamed from .exe)
          - Version.info - Generated by the PatchCreator, contains the checksum of the new patcher and the version to patch from.

In FileZilla Server, add a user called "anonymous", with no authentication required.

Go to Passive Mode settings, and set custom ports 50000-51000, and also set the public IP.

Open up all the ports for all inbound IPv6 and IPv4 in the Firewall and on your AWS Security Group Policy.

Server setting: Require explicit FTP over TLS
Client: Use explicit FTP over TLS if available
