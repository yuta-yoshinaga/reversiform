////////////////////////////////////////////////////////////////////////////////
///	@file			ReversiConst.cs
///	@brief			アプリ定数クラス
///	@author			Yuta Yoshinaga
///	@date			2017.10.20
///	$Version:		$
///	$Revision:		$
///
/// Copyright (c) 2017 Yuta Yoshinaga. All Rights reserved.
///
/// - 本ソフトウェアの一部又は全てを無断で複写複製（コピー）することは、
///   著作権侵害にあたりますので、これを禁止します。
/// - 本製品の使用に起因する侵害または特許権その他権利の侵害に関しては
///   当社は一切その責任を負いません。
///
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReversiForm
{
	////////////////////////////////////////////////////////////////////////////////
	///	@class		ReversiConst
	///	@brief		アプリ設定クラス
	///
	////////////////////////////////////////////////////////////////////////////////
	static class ReversiConst
	{
		#region 定数
		public const int DEF_MODE_ONE = 0;								//!< 一人対戦
		public const int DEF_MODE_TWO = 1;								//!< 二人対戦

		public const int DEF_TYPE_EASY = 0;								//!< CPU 弱い
		public const int DEF_TYPE_NOR = 1;								//!< CPU 普通
		public const int DEF_TYPE_HARD = 2;								//!< CPU 強い

		public const int DEF_COLOR_BLACK = 0;							//!< コマ色 黒
		public const int DEF_COLOR_WHITE = 1;							//!< コマ色 白

		public const int DEF_ASSIST_OFF = 0;							//!< アシスト OFF
		public const int DEF_ASSIST_ON = 1;								//!< アシスト ON

		public const int DEF_GAME_SPD_FAST = 0;							//!< ゲームスピード 早い
		public const int DEF_GAME_SPD_MID = 1;							//!< ゲームスピード 普通
		public const int DEF_GAME_SPD_SLOW = 2;							//!< ゲームスピード 遅い

		public const int DEF_GAME_SPD_FAST_VAL = 0;						//!< ゲームスピード 早い
		public const int DEF_GAME_SPD_MID_VAL = 50;						//!< ゲームスピード 普通
		public const int DEF_GAME_SPD_SLOW_VAL = 100;					//!< ゲームスピード 遅い

		public const int DEF_GAME_SPD_FAST_VAL2 = 0;					//!< ゲームスピード 早い
		public const int DEF_GAME_SPD_MID_VAL2 = 500;					//!< ゲームスピード 普通
		public const int DEF_GAME_SPD_SLOW_VAL2 = 1000;					//!< ゲームスピード 遅い

		public const int DEF_END_ANIM_OFF = 0;							//!< 終了アニメーション OFF
		public const int DEF_END_ANIM_ON = 1;							//!< 終了アニメーション ON

		public const int DEF_MASU_CNT_6 = 0;							//!< マス縦横6
		public const int DEF_MASU_CNT_8 = 1;							//!< マス縦横8
		public const int DEF_MASU_CNT_10 = 2;							//!< マス縦横10
		public const int DEF_MASU_CNT_12 = 3;							//!< マス縦横12
		public const int DEF_MASU_CNT_14 = 4;							//!< マス縦横14
		public const int DEF_MASU_CNT_16 = 5;							//!< マス縦横16
		public const int DEF_MASU_CNT_18 = 6;							//!< マス縦横18
		public const int DEF_MASU_CNT_20 = 7;							//!< マス縦横20

		public const int DEF_MASU_CNT_6_VAL = 6;						//!< マス縦横6
		public const int DEF_MASU_CNT_8_VAL = 8;						//!< マス縦横8
		public const int DEF_MASU_CNT_10_VAL = 10;						//!< マス縦横10
		public const int DEF_MASU_CNT_12_VAL = 12;						//!< マス縦横12
		public const int DEF_MASU_CNT_14_VAL = 14;						//!< マス縦横14
		public const int DEF_MASU_CNT_16_VAL = 16;						//!< マス縦横16
		public const int DEF_MASU_CNT_18_VAL = 18;						//!< マス縦横18
		public const int DEF_MASU_CNT_20_VAL = 20;						//!< マス縦横20
		public const int DEF_MASU_CNT_MAX_VAL = DEF_MASU_CNT_20_VAL;	//!< マス縦横最大

		public const int REVERSI_STS_NONE = 0;							//!< コマ無し
		public const int REVERSI_STS_BLACK = 1;							//!< 黒
		public const int REVERSI_STS_WHITE = 2;							//!< 白
		public const int REVERSI_STS_MIN = 0;							//!< ステータス最小値
		public const int REVERSI_STS_MAX = 2;							//!< ステータス最大値
		public const int REVERSI_MASU_CNT = 8;							//!< 縦横マス数

		public const int LC_MSG_DRAW = 0;								//!< マス描画
		public const int LC_MSG_ERASE = 1;								//!< マス消去
		public const int LC_MSG_DRAW_INFO = 2;							//!< マス情報描画
		public const int LC_MSG_ERASE_INFO = 3;							//!< マス情報消去
		public const int LC_MSG_DRAW_ALL = 4;							//!< 全マス描画
		public const int LC_MSG_ERASE_ALL = 5;							//!< 全マス消去
		public const int LC_MSG_DRAW_INFO_ALL = 6;						//!< 全マス情報描画
		public const int LC_MSG_ERASE_INFO_ALL = 7;						//!< 全マス情報消去
		public const int LC_MSG_DRAW_END = 8;							//!< 描画終わり
		public const int LC_MSG_CUR_COL = 9;							//!< 現在の色
		public const int LC_MSG_CUR_COL_ERASE = 10;						//!< 現在の色消去
		public const int LC_MSG_CUR_STS = 11;							//!< 現在のステータス
		public const int LC_MSG_CUR_STS_ERASE = 12;						//!< 現在のステータス消去
		public const int LC_MSG_MSG_DLG = 13;							//!< メッセージダイアログ
		public const int LC_MSG_DRAW_ALL_RESET = 14;					//!< 全マスビットマップインスタンスクリア
		#endregion
	}
}
