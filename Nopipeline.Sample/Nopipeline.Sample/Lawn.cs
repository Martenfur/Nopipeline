using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Nopipeline.Sample
{
  public class Lawn
  {

		private Texture2D _animal;
		private Texture2D _flower;
		private Texture2D _tree;
		private Texture2D _boulder;

		private Random _random = new Random();

		private List<LawnItem> _items = new List<LawnItem>();

		private readonly Vector2 _minLawnPosition = new Vector2(0, 0);
		private readonly Vector2 _maxLawnPosition = new Vector2(700, 400);

		public void Init(ContentManager content)
		{
			_animal = content.Load<Texture2D>("Graphics/Animal");
			_flower = content.Load<Texture2D>("Graphics/Flower");
			_tree = content.Load<Texture2D>("Graphics/Tree");
			_boulder = content.Load<Texture2D>("Graphics/Boulder");

			for(var i = 0; i < 20; i += 1)
			{ 
				_items.Add(
					new LawnItem(
						_flower,
						GetRandomPosition(),
						GetRandomVerticalFlipFlags() | GetRandomHorizontalFlipFlags()
					)
				);
			}			
			
			for(var i = 0; i < 10; i += 1)
			{ 
				_items.Add(
					new LawnItem(
						_tree,
						GetRandomPosition(),
						GetRandomHorizontalFlipFlags()
					)
				);
			}			
			
			for(var i = 0; i < 5; i += 1)
			{ 
				_items.Add(
					new LawnItem(
						_boulder,
						GetRandomPosition(),
						GetRandomHorizontalFlipFlags()
					)
				);
			}			
			
			for(var i = 0; i < 10; i += 1)
			{ 
				_items.Add(
					new LawnItem(
						_animal,
						GetRandomPosition(),
						GetRandomHorizontalFlipFlags()
					)
				);
			}
		}

		public void Draw()
		{ 
			foreach(var item in _items)
			{ 
				item.Draw();
			}
		}

		private Vector2 GetRandomPosition()
		{
			return new Vector2(
				_random.Next((int)_minLawnPosition.X, (int)_maxLawnPosition.X), 
				_random.Next((int)_minLawnPosition.Y, (int)_maxLawnPosition.Y)
			);
		}
		
		private SpriteEffects GetRandomVerticalFlipFlags()
		{ 
			if (_random.NextDouble() > 0.5)
			{ 
				return SpriteEffects.FlipVertically;
			}
			return SpriteEffects.None;
		}
		
		private SpriteEffects GetRandomHorizontalFlipFlags()
		{ 
			if (_random.NextDouble() > 0.5)
			{ 
				return SpriteEffects.FlipHorizontally;
			}
			return SpriteEffects.None;
		}
		
  }
}
