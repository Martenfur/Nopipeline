using Microsoft.Xna.Framework.Content;
using Monofoxe.Engine;
using Monofoxe.Tiled.MapStructure;


namespace Resources {
	public static class Maps {
		private static ContentManager _content;

		public static TiledMap Level1;
		public static TiledMap Level2;

		public static void Load() {
			_content = new ContentManager(GameMgr.Game.Services);
			_content.RootDirectory = AssetMgr.ContentDir + '/' + AssetMgr.MapsDir;

			Level1 = _content.Load<TiledMap>("lvl3");
			Level2 = _content.Load<TiledMap>("lvl2");

		}

		public static void Unload() {
			_content.Unload();
		}

	}
}
