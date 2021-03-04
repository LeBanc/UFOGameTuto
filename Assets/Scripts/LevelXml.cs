using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

// Serialization is not in "Level" class because Unity tries to serialize all inhereted data of Monobehaviour

[XmlRoot("Level")]
public class LevelXml
{
    [XmlAttribute("name")]
    public string Name;

    public int Width;
    public int Height;

    [XmlArray("Blocks"), XmlArrayItem("Block")]
    public List<Position> Blocks;

    [XmlArray("PickupSpawns"), XmlArrayItem("Pickups")]
    public List<Position> PickUpSpawns;

    [XmlArray("UFOSpawns"), XmlArrayItem("UFOSpawn")]
    public Position[] UFOSpawns = new Position[4];


    public void SaveToXml(Level level, string path)
    {
        Name = level.gameObject.name;
        Width = level.width;
        Height = level.height;
        Blocks = level.blocksPositions;
        PickUpSpawns = level.pickUpPositions;
        UFOSpawns = level.ufoPositions;

        var serializer = new XmlSerializer(typeof(LevelXml));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public void LoadFromXml(string path, Level level)
    {
        LevelXml xml = new LevelXml();
        var serializer = new XmlSerializer(typeof(LevelXml));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            xml = serializer.Deserialize(stream) as LevelXml;
        }
        level.gameObject.name = xml.Name;
        level.width = xml.Width;
        level.height = xml.Height;
        level.blocksPositions = xml.Blocks;
        level.pickUpPositions = xml.PickUpSpawns;
        level.ufoPositions = xml.UFOSpawns;
    }
}
