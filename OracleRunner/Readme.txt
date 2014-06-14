The oracle client driver setup is notorious and frustrating at times, even if the .net development libraries can be xcopy deployed, the client can still be painful to manage.

Scavenging through the various efforts of others who have shared their findings, 
it all comes to the following set of dlls required in addition to the reference to the Oracle.DataAccess library.

You can replace the files with your respective version, only the version number should be changing in the names.

oci.dll
ociw32.dll
orannzsbb11.dll
oraocci11.dll
oraociei11.dll
oraociicus11.dll
OraOps11w.dll

The oraocieixx.dll file is very large, its about 120MB for the v11 release, surpassing the github limit of 100MB for files.

So, the binaries now exist in a zip - OracleBinaries.

