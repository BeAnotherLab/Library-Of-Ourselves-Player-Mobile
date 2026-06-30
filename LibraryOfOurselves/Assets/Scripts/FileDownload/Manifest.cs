[System.Serializable]
public class Manifest
{
    public int version;
    public ManifestFile[] files;
}

[System.Serializable]
public class ManifestFile
{
    public string path;
    public long size;
    public string sha256;
}