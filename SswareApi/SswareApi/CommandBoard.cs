using Helper;
using System.Data;

namespace SswareApi
{
    public partial class Command
    {
        public class BoardManagerClass : XObjectManagerClass
        {
            public BoardManagerClass(Command command) : base(command)
            {

            }

            #region Manage Board

            public Board CreateBoard(string name)
            {
                Verification(UserType.ItAdministrator);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 1, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new Board(GetObjectInfo((int)XObjectType.Board
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableBoard(int id, bool enable)
            {
                Verification(UserType.ItAdministrator);

                EnableDisableObject(6, id, enable);
            }

            public void DeleteBoard(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(6, id);
            }

            #endregion

            #region Manage BoardItem

            public BoardItem CreateBoardItem(string name, string body, int board)
            {
                Verification(UserType.Employee);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 101, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@body", body, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@board", board, SqlDbType.Int);
                Command.DbHelper.AddInput("@registeredBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardItem(GetObjectInfo((int)XObjectType.BoardItem
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public BoardItem UpdateBoardItem(BoardItem boardItem)
            {
                Verification(UserType.Employee, XObjectType.BoardItem, boardItem.Id);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 102, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", boardItem.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@namez", boardItem.Name, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@body", boardItem.Body, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@updatedBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardItem(GetObjectInfo((int)XObjectType.BoardItem
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableBoardItem(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.BoardItem, id);

                EnableDisableObject(7, id, enable);
            }

            public void DeleteBoardItem(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(7, id);
            }

            #endregion

            #region Manage BoardComment

            public BoardComment CreateBoardComment(string body, int boardItem, int parent)
            {
                Verification(UserType.Employee);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 201, SqlDbType.Int);
                Command.DbHelper.AddInput("@body", body, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@boardItem", boardItem, SqlDbType.Int);
                Command.DbHelper.AddInput("@parent", parent, SqlDbType.Int);
                Command.DbHelper.AddInput("@registeredBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardComment(GetObjectInfo((int)XObjectType.BoardComment
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public BoardComment UpdateBoardComment(BoardComment boardComment)
            {
                Verification(UserType.Employee, XObjectType.BoardComment, boardComment.Id);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 202, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", boardComment.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@body", boardComment.Body, SqlDbType.NVarChar);
                Command.DbHelper.AddInput("@updatedBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardComment(GetObjectInfo((int)XObjectType.BoardComment
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableBoardComment(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.BoardComment, id);

                EnableDisableObject(8, id, enable);
            }

            public void DeleteBoardComment(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(8, id);
            }

            #endregion

            #region Manage BoardVote

            public BoardVote CreateBoardVote(int value, int boardItem, int boardComment)
            {
                Verification(UserType.Employee);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 301, SqlDbType.Int);
                Command.DbHelper.AddInput("@valuez", value, SqlDbType.Int);
                Command.DbHelper.AddInput("@boardItem", boardItem, SqlDbType.Int);
                Command.DbHelper.AddInput("@boardComment", boardComment, SqlDbType.Int);
                Command.DbHelper.AddInput("@registeredBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardVote(GetObjectInfo((int)XObjectType.BoardVote
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public BoardVote UpdateBoardVote(BoardVote boardVote)
            {
                Verification(UserType.Employee, XObjectType.BoardVote, boardVote.Id);

                DataSet ds = new DataSet();
                Command.DbHelper.AddInput("@operation", 302, SqlDbType.Int);
                Command.DbHelper.AddInput("@id", boardVote.Id, SqlDbType.Int);
                Command.DbHelper.AddInput("@valuez", boardVote.Value, SqlDbType.Int);
                Command.DbHelper.AddInput("@updatedBy", Command.CurrentUser.Id, SqlDbType.Int);
                Command.DbHelper.StoredProcedure("p_BoardManager", ds);

                return new BoardVote(GetObjectInfo((int)XObjectType.BoardVote
                    , (int)ds.Tables[0].Rows[0][0]));
            }

            public void EnableDisableBoardVote(int id, bool enable)
            {
                Verification(UserType.Employee, XObjectType.BoardVote, id);

                EnableDisableObject(9, id, enable);
            }

            public void DeleteBoardVote(int id)
            {
                Verification(UserType.ItAdministrator);

                DeleteObject(9, id);
            }

            #endregion
        }
    }
}