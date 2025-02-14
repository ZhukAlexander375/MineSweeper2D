using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Create Tile Set")]
public class TileSetConfig : ScriptableObject
{
    public Tile TileInactive;
    public Tile TileActive;
    public Tile TileEmpty;
    public Tile TileMine;
    public Tile TileExploded;
    public Tile TileFlag;
    public Tile TileNum1;
    public Tile TileNum2;
    public Tile TileNum3;
    public Tile TileNum4;
    public Tile TileNum5;
    public Tile TileNum6;
    public Tile TileNum7;
    public Tile TileNum8;
    public Tile TileInactivePrize;
    public Tile TileActivePrize;

    //FOR TESTS
    public Tile TileMineVisible;
}
