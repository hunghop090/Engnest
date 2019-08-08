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
		List<CommentViewModel> LoadCommentsPost(string PostIds,string date,int quantity,long LoginUser);

		List<CommentViewModel> LoadCommentsPost(string PostIds,string date,int quantity);

		List<CommentViewModel> LoadCommentsReply(string CommentId,string date,int quantity,string createdUser,long LoginUser);
        
		List<CommentViewModel> LoadCommentsReply(string CommentId,string date,int quantity,string createdUser);
		CommentViewModel GetCommentByID(long CommentId,long LoginUser);

		Comment GetCommentByUpdate(long CommentId);
        List<Comment> GetCommentByTargetId(long TargetId);
        long InsertComment(Comment Comment);
        void DeleteComment(long CommentID);
        void UpdateComment(Comment Comment);
        void Save();
	}
}