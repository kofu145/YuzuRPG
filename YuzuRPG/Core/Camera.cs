namespace YuzuRPG.Core;

public class Camera
{
    private Player hookedPlayer;
    private char[][] view;
    public int Width;
    public int Height;

    public Camera(Player player, int width, int height)
    {
        hookedPlayer = player;
        this.Width = width;
        this.Height = height;
    }

    public char[][] GetRender(char[][] map)
    {
        if (map == null)
        {
            throw new InvalidOperationException("Map is null for whatever reason!");
        }
        view = new char[Height][];
        for (int dy = 0; dy < view.Length; dy++)
        {            
            view[dy] = new char[Width];
            for (int dx = 0; dx < view[0].Length; dx++)
            {
                // since we render from a corner, this is to center it (to be focused on player position)
                var renderY = hookedPlayer.Y - Height / 2;
                var renderX = hookedPlayer.X - Width / 2;
                
                if (renderY + dy >= map.Length || renderX + dx >= map[0].Length ||
                    renderY + dy < 0 || renderX + dx < 0)
                    view[dy][dx] = ' ';
                else
                    view[dy][dx] = map[renderY + dy][renderX + dx];
            }
        }

        return view;
    }
}