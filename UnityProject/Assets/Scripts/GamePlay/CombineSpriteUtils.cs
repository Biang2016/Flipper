using System;
using UnityEngine;

class CombineSpriteUtils
{
    public static Sprite CombineSprite(int width, int height, Sprite startSprite, Sprite endSprite)
    {
        Rect rect = new Rect(startSprite.rect.xMin, endSprite.rect.yMin, endSprite.rect.xMax - startSprite.rect.xMin, startSprite.rect.yMax - endSprite.rect.yMin);
        Sprite res = Sprite.Create(startSprite.texture, rect, Vector2.one * 0.5f, startSprite.pixelsPerUnit);
        return res;
    }

    static Texture2D GetTextureFromSprite(Sprite sprite)
    {
        Rect rect = sprite.textureRect;
        Texture2D texture2D = sprite.texture;
        Texture2D slicedTexture = new Texture2D((int) rect.width, (int) rect.height);

        for (int y = (int) rect.y; y < rect.y + rect.height; y++) //Y轴像素  
        {
            for (int x = (int) rect.x; x < rect.x + rect.width; x++)
            {
                slicedTexture.SetPixel(x - (int) rect.x, y - (int) rect.y, texture2D.GetPixel(x, y));
            }
        }

        return slicedTexture;
    }
}