////////////////////////////////////////////////////////////////////////////////
///	@file			FormMain.cs
///	@brief			FormMainクラス
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ReversiForm
{
	////////////////////////////////////////////////////////////////////////////////
	///	@class		FormMain
	///	@brief		FormMainクラス
	///
	////////////////////////////////////////////////////////////////////////////////
	public partial class Reversi : Form
	{
		delegate void ViewMsgDlgDelegate(string title , string msg);
		delegate void DrawSingleDelegate(int y, int x, int sts, int bk, string text);
		delegate void CurColMsgDelegate(string text);
		delegate void CurStsMsgDelegate(string text);
		delegate void Reversi_ResizeEndDelegate(object sender, EventArgs e);

		public ReversiSetting m_AppSettings;								//!< アプリ設定
		public ReversiPlay m_ReversiPlay;									//!< リバーシ本体
		private static System.Timers.Timer aTimer;							//!< タイマー

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			コンストラクタ
		///	@fn				FormMain()
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public Reversi()
		{
			InitializeComponent();

			Assembly myAssembly = Assembly.GetEntryAssembly();
			string setPath = System.IO.Path.GetDirectoryName(myAssembly.Location) + "\\" + "AppSetting.xml";
			try
			{
				this.m_AppSettings = this.LoadSettingXml(setPath);
				if(this.m_AppSettings == null)
				{
					this.m_AppSettings = new ReversiSetting();
					this.SaveSettingXml(setPath,ref this.m_AppSettings);
				}
				this.m_ReversiPlay = new ReversiPlay();
				this.m_ReversiPlay.mSetting = this.m_AppSettings;
				this.m_ReversiPlay.viewMsgDlg = this.ViewMsgDlg;
				this.m_ReversiPlay.drawSingle = this.DrawSingle;
				this.m_ReversiPlay.curColMsg = this.CurColMsg;
				this.m_ReversiPlay.curStsMsg = this.CurStsMsg;
				this.appInit();
				Task newTask = new Task( () => { this.m_ReversiPlay.reset(); } );
				newTask.Start();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("FormMain(1) : " + ex.Message);
				System.Console.WriteLine("FormMain(1) : " + ex.StackTrace);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			設定XMLファイルロード
		///	@fn				ReversiSetting LoadSettingXml(string path)
		///	@param[in]		string path		設定XMLファイルパス
		///	@return			ReversiSettingオブジェクトインスタンス
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public ReversiSetting LoadSettingXml(string path)
		{
			ReversiSetting ret = null;
			try
			{
				// *** XMLをReversiSettingオブジェクトに読み込む *** //
				XmlSerializer serializer = new XmlSerializer(typeof(ReversiSetting));
				ret = new ReversiSetting();

				FileStream fsr = new FileStream(path, FileMode.Open);

				// *** XMLファイルを読み込み、逆シリアル化（復元）する *** //
				ret = (ReversiSetting)serializer.Deserialize(fsr);
				fsr.Close();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("LoadSettingXml() : " + ex.Message);
				System.Console.WriteLine("LoadSettingXml() : " + ex.StackTrace);
				ret = null;
			}
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			設定XMLファイルセーブ
		///	@fn				int SaveSettingXml(string path,ReversiSetting appSet)
		///	@param[in]		string path			設定XMLファイルパス
		///	@param[out]		ReversiSetting appSet	設定XMLファイルオブジェクト
		///	@return			0 : 成功 それ以外 : 失敗
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public int SaveSettingXml(string path,ref ReversiSetting appSet)
		{
			int ret = 0;
			try
			{
				// *** XMLをReversiSettingオブジェクトに読み込む *** //
				XmlSerializer serializer = new XmlSerializer(typeof(ReversiSetting));

				// *** カレントディレクトリに"AppSetting.xml"というファイルで書き出す *** //
				FileStream fsw = new FileStream(path, FileMode.Create);

				// *** オブジェクトをシリアル化してXMLファイルに書き込む *** //
				serializer.Serialize(fsw, appSet);
				fsw.Close();
			}
			catch (Exception exl)
			{
				System.Console.WriteLine("SaveSettingXml() : " + exl.Message);
				System.Console.WriteLine("SaveSettingXml() : " + exl.StackTrace);
				ret = -1;
			}
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			アプリ初期化
		///	@fn				void appInit()
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void appInit()
		{
			Size curSize = this.tableLayoutPanel1.Size;
			int cellSizeAll = curSize.Height;
			if (curSize.Width < cellSizeAll) cellSizeAll = curSize.Width;
			curSize.Height = cellSizeAll;
			curSize.Width = cellSizeAll;
			int cellSize = cellSizeAll / this.m_AppSettings.mMasuCnt;
			float per = (float)cellSize / cellSizeAll * 100;
			this.tableLayoutPanel1.Visible = false;
			for (int i = 0; i < ReversiConst.DEF_MASU_CNT_MAX_VAL;i++)
			{
				for (int j = 0; j < ReversiConst.DEF_MASU_CNT_MAX_VAL;j++)
				{
					int curIdx = (i * ReversiConst.DEF_MASU_CNT_MAX_VAL) + j + 1;
					string curIdxStr = "pictureBox" + curIdx.ToString();
					Control c = this.tableLayoutPanel1.Controls[curIdxStr];
					if (c != null)
					{
						if( i < this.m_AppSettings.mMasuCnt && j < this.m_AppSettings.mMasuCnt)
						{
							c.Visible = true;
						}
						else
						{
							c.Visible = false;
						}
					}
					// *** テーブルの列サイズを調整 *** //
					if(j < this.m_AppSettings.mMasuCnt)
					{
						this.tableLayoutPanel1.ColumnStyles[j] = new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, per);
					}
					else
					{
						this.tableLayoutPanel1.ColumnStyles[j] = new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0F);
					}
				}
				// *** テーブルの行サイズを調整 *** //
				if(i < this.m_AppSettings.mMasuCnt)
				{
					this.tableLayoutPanel1.RowStyles[i] = new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, per);
				}
				else
				{
					this.tableLayoutPanel1.RowStyles[i] = new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F);
				}
			}
			this.tableLayoutPanel1.Visible = true;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			メッセージダイアログ
		///	@fn				void ViewMsgDlg(string title , string msg)
		///	@param[in]		string title	タイトル
		///	@param[in]		string msg		メッセージ
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void ViewMsgDlg(string title , string msg)
		{
			Invoke(new ViewMsgDlgDelegate(ViewMsgDlgLocal), title, msg);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			メッセージダイアログ
		///	@fn				void ViewMsgDlgLocal(string title , string msg)
		///	@param[in]		string title	タイトル
		///	@param[in]		string msg		メッセージ
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void ViewMsgDlgLocal(string title , string msg)
		{
			MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			1マス描画
		///	@fn				void DrawSingle(int y, int x, int sts, int bk, string text)
		///	@param[in]		int y		Y座標
		///	@param[in]		int x		X座標
		///	@param[in]		int sts		ステータス
		///	@param[in]		int bk		背景
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void DrawSingle(int y, int x, int sts, int bk, string text)
		{
			Invoke(new DrawSingleDelegate(DrawSingleLocal), y, x, sts, bk, text);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			1マス描画
		///	@fn				void DrawSingleLocal(int y, int x, int sts, int bk, string text)
		///	@param[in]		int y		Y座標
		///	@param[in]		int x		X座標
		///	@param[in]		int sts		ステータス
		///	@param[in]		int bk		背景
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void DrawSingleLocal(int y, int x, int sts, int bk, string text)
		{
			PictureBox curPict = (PictureBox) this.tableLayoutPanel1.GetControlFromPosition(x,y);
			if(curPict != null)
			{
				// 描画先とするImageオブジェクトを作成する
				Bitmap canvas = new Bitmap(curPict.Width, curPict.Height);
				// ImageオブジェクトのGraphicsオブジェクトを作成する
				Graphics g = Graphics.FromImage(canvas);
				g.SmoothingMode = SmoothingMode.HighQuality;
				Pen curPen1 = new Pen(m_AppSettings.mBorderColor,2);
				// Brushオブジェクトの作成
				SolidBrush curBru1 = null;
				SolidBrush curBru2 = null;
				SolidBrush curBru3 = null;
				Color curBkColor = m_AppSettings.mBackGroundColor;
				byte tmpA = curBkColor.A;
				byte tmpR = curBkColor.R;
				byte tmpG = curBkColor.G;
				byte tmpB = curBkColor.B;
				Color curBkColorRev;

				if (bk == 1) {
					// *** cell_back_blue *** //
					tmpG -= 127;
					if (tmpG < 0) tmpG += 255;
					tmpB += 255;
					if (255 < tmpB) tmpB -= 255;
					curBkColor = Color.FromArgb(tmpA, tmpR, tmpG, tmpB);
				}
				else if (bk == 2) {
					// *** cell_back_red *** //
					tmpR += 255;
					if(255 < tmpR) tmpR -= 255;
					tmpG -= 127;
					if(tmpG < 0) tmpG += 255;
					tmpB -= 127;
					if(tmpB < 0) tmpB += 255;
					curBkColor = Color.FromArgb(tmpA, tmpR, tmpG, tmpB);
			    } else if (bk == 3) {
					// *** cell_back_magenta *** //
					tmpR += 255;
					if(255 < tmpR) tmpR -= 255;
					tmpB += 255;
					if(255 < tmpB) tmpB -= 255;
					curBkColor = Color.FromArgb(tmpA, tmpR, tmpG, tmpB);
			    } else {
					// *** cell_back_green *** //
					curBkColor = Color.FromArgb(tmpA, tmpR, tmpG, tmpB);
			    }
				curBru1 = new SolidBrush(curBkColor);
				HslColor workCol = HslColor.FromRgb(curBkColor);
				float h = workCol.H + 180F;
				if (359F < h) h -= 360F;
				curBkColorRev = HslColor.ToRgb(new HslColor(h, workCol.S, workCol.L));
				curBru3 = new SolidBrush(curBkColorRev);

				if (sts == ReversiConst.REVERSI_STS_NONE)
				{
				}
				else if (sts == ReversiConst.REVERSI_STS_BLACK)
				{
					curBru2 = new SolidBrush(m_AppSettings.mPlayerColor1);
				}
				else if (sts == ReversiConst.REVERSI_STS_WHITE)
				{
					curBru2 = new SolidBrush(m_AppSettings.mPlayerColor2);
				}

				// 位置(x, y)にWidth x Heightの四角を描く
				g.FillRectangle(curBru1, 0, 0, curPict.Width, curPict.Height);
				// 位置(x, y)にWidth x Heightの四角を描く
				g.DrawRectangle(curPen1, 0, 0, curPict.Width, curPict.Height);
				// 先に描いた四角に内接する楕円を黒で描く
				if(curBru2 != null) g.FillEllipse(curBru2, 2, 2, curPict.Width - 4, curPict.Height - 4);

				if (text != null && text.Length != 0 && text != "0")
				{
					// フォントオブジェクトの作成
					int fntSize = curPict.Width;
					if(curPict.Height < fntSize) fntSize = curPict.Height;
					fntSize = (int)((double)fntSize * 0.75);
					fntSize /= text.Length;
					if (fntSize < 8) fntSize = 8;
					Font fnt = new Font("MS UI Gothic", fntSize);
					Rectangle rect1 = new Rectangle(0, 0, curPict.Width, curPict.Height);
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					g.DrawString(text, fnt, curBru3, rect1, stringFormat);
					//リソースを解放する
					fnt.Dispose();
				}
				// リソースを解放する
				g.Dispose();
				// curPictに表示する
				curPict.Image = canvas;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在の色メッセージ
		///	@fn				void CurColMsg(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurColMsg(string text)
		{
			Invoke(new CurColMsgDelegate(CurColMsgLocal), text);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在の色メッセージ
		///	@fn				void CurColMsgLocal(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurColMsgLocal(string text)
		{
			this.label1.Text = text;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在のステータスメッセージ
		///	@fn				void CurStsMsg(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurStsMsg(string text)
		{
			Invoke(new CurStsMsgDelegate(CurStsMsgLocal), text);
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			現在のステータスメッセージ
		///	@fn				void CurStsMsgLocal(string text)
		///	@param[in]		string text	テキスト
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		public void CurStsMsgLocal(string text)
		{
			this.label2.Text = text;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			マスクリック
		///	@fn				void pictureBox_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void pictureBox_Click(object sender, EventArgs e)
		{
			TableLayoutPanelCellPosition pos = this.tableLayoutPanel1.GetCellPosition((Control)sender);

			Console.WriteLine("click y=" + pos.Row + " x=" + pos.Column);

			Task newTask = new Task( () => { this.m_ReversiPlay.reversiPlay(pos.Row, pos.Column); } );
			newTask.Start();
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			リセットクリック
		///	@fn				void button1_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void button1_Click(object sender, EventArgs e)
		{
			this.appInit();
			Task newTask = new Task( () => { this.m_ReversiPlay.reset(); } );
			newTask.Start();
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			セッティングクリック
		///	@fn				void button2_Click(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void button2_Click(object sender, EventArgs e)
		{
			Assembly myAssembly = Assembly.GetEntryAssembly();
			string setPath = System.IO.Path.GetDirectoryName(myAssembly.Location) + "\\" + "AppSetting.xml";

			SettingForm form = new SettingForm(this.m_AppSettings);
			// *** オーナーウィンドウにthisを指定する *** //
			form.ShowDialog(this);

			this.m_AppSettings = form.mSetting;
			SaveSettingXml(setPath,ref this.m_AppSettings);

			// *** フォームが必要なくなったところで、Disposeを呼び出す *** //
			form.Dispose();
			this.m_ReversiPlay.mSetting = this.m_AppSettings;
			this.appInit();
			Task newTask = new Task( () => { this.m_ReversiPlay.reset(); } );
			newTask.Start();
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			リサイズイベント
		///	@fn				void Reversi_Resize(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void Reversi_Resize(object sender, EventArgs e)
		{
			System.Console.WriteLine("Reversi_Resize() : ");

			// Create a timer with a 1.5 second interval.
			double interval = 1000.0;
            if(aTimer != null)
            {
                // *** タイマーキャンセル *** //
                aTimer.Enabled = false;
            }
			aTimer = new System.Timers.Timer(interval);
			// Hook up the event handler for the Elapsed event.
			aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			// Only raise the event the first time Interval elapses.
			aTimer.AutoReset = false;
			aTimer.Enabled = true;
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			リサイズ終了イベント
		///	@fn				void Reversi_Resize(object sender, EventArgs e)
		///	@param[in]		object sender
		///	@param[in]		EventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void Reversi_ResizeEnd(object sender, EventArgs e)
		{
			// *** リサイズ終了後、再描画とレイアウトロジックを実行する *** //
			System.Console.WriteLine("Reversi_ResizeEnd() : ");
			this.Invalidate();
			this.PerformLayout();
			this.appInit();
			Task newTask = new Task( () => { this.m_ReversiPlay.drawUpdateForcibly(this.m_AppSettings.mAssist); } );
			newTask.Start();
		}

		////////////////////////////////////////////////////////////////////////////////
		///	@brief			タイマーイベント
		///	@fn				void OnTimedEvent(object source, ElapsedEventArgs e)
		///	@param[in]		object source
		///	@param[in]		ElapsedEventArgs e
		///	@return			ありません
		///	@author			Yuta Yoshinaga
		///	@date			2017.10.20
		///
		////////////////////////////////////////////////////////////////////////////////
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			Invoke(new Reversi_ResizeEndDelegate(this.Reversi_ResizeEnd), source, e);
		}
	}
}
