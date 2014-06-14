The InformixBinaries directory contains the files required for Informix client to run without an explicit installation.

You can copy the structure in the InformixBinaries directory to any directory and set it up from that location by executing SetupClient.cmd file.

1) cd to the directory where the InformixBinaries structure is copied to.
2) execute SetupClient.cmd file.
    The cmd file will add a set of registry entries (administrator privileges required).
    It sets a path entry to the location it is running from.
    It sets an environment variable to point to the location its running from.
3) The en_us\ibm.data.informix.xml is required in your application to translate message strings. For different countries + languages, include the respective xml.
4) msg\en_us also follows the same pattern as above, replace with your respective language directory.
