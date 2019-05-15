using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engnest.Entities.IRepository
{
	public interface  ICommentRepository : IDisposable
	{
        List<Comment> GetComments();
		List<CommentViewModel> LoadCommentsPost(string PostIds);
        Comment GetCommentByID(long CommentId);
        List<Comment> GetCommentByTargetId(long TargetId);
        void InsertComment(Comment Comment);
        void DeleteComment(long CommentID);
        void UpdateComment(Comment Comment);
        void Save();
	}
}