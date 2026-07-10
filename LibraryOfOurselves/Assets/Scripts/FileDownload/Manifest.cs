[System.Serializable]
public class Manifest
{
    public int version;
    public ManifestFile[] guideFiles;
    public ManifestFile[] userFiles;
}

[System.Serializable]
public class ManifestFile
{
    public string filename;
    public long size;
    public string sha256;
}