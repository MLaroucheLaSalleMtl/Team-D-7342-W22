using System.Collections;


public interface ITilemapListener
{
    public void OnAddedTile(string tilemapName);
    public void OnRemovedTile(string tilemapName);
}
