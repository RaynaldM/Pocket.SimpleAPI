using System;
using System.Collections.Generic;

namespace Pocket.api
{
    public class AuthRequest
    {
        public string consumer_key { get; set; }
        public string redirect_uri { get; set; }
    }

    public class AuthRequestResult
    {
        public string code { get; set; }
    }

    public class PocketAccessTokenRequest
    {
        public string consumer_key { get; set; }
        public string code { get; set; }
    }

    public class PocketAccessToken
    {
        public string access_token { get; set; }
        public string username { get; set; }
    }

    public class Image
    {
        public string item_id { get; set; }
        public string image_id { get; set; }
        public string src { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string credit { get; set; }
        public string caption { get; set; }
    }

    public class Images
    {
        public List<Image> Image { get; set; }
    }

    public class Video
    {
        public string item_id { get; set; }
        public string video_id { get; set; }
        public string src { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string type { get; set; }
        public string vid { get; set; }
    }

    public class Videos
    {
        public List<Video> Video { get; set; }
    }

    public class Item
    {
        public string item_id { get; set; }
        public string resolved_id { get; set; }
        public string given_url { get; set; }
        public string given_title { get; set; }
        public string favorite { get; set; }
        public string status { get; set; }
        public string time_added { get; set; }
        public string time_updated { get; set; }
        public string time_read { get; set; }
        public string time_favorited { get; set; }
        public int sort_id { get; set; }
        public string resolved_title { get; set; }
        public string resolved_url { get; set; }
        public string excerpt { get; set; }
        public string is_article { get; set; }
        public string is_index { get; set; }
        public string has_video { get; set; }
        public string has_image { get; set; }
        public string word_count { get; set; }
        public Image image { get; set; }

        //public Images images { get; set; }
        //public Videos videos { get; set; }
    }

    public class AuthBaseRequest
    {
        public string consumer_key { get; set; }
        public string access_token { get; set; }
    }

    public class RetrieveRequest : AuthBaseRequest
    {
        public string state { get; set; }
        public string count { get; set; }
        public string detailType { get; set; }
        public string tag { get; set; }
        public string sort { get; set; }
        public string search { get; set; }
        public long since { get; set; }
        public int offset { get; set; }
    }

    public class RetrieveResult
    {
        public int status { get; set; }
        public int complete { get; set; }
        public object error { get; set; }
        public SearchMeta search_meta { get; set; }
        public int since { get; set; }
        public List<Item> list { get; set; }
    }

    public class SearchMeta
    {
        public string search_type { get; set; }
    }

    public class ArticleRequest : AuthBaseRequest
    {
        public string url { get; set; }
        public string title { get; set; }
        public string tags { get; set; }
        public string tweet_id { get; set; }
    }

    public class AddResult
    {
        public Item item { get; set; }
        public int status { get; set; }
    }

    public class ModifyRequest : AuthBaseRequest
    {
        public List<ModifyAction> actions { get; set; }
    }

    public class ModifyResult
    {
        public List<bool> action_results { get; set; }
        public int status { get; set; }
    }

    public class SingleModifyResult
    {
        public bool action_result { get; set; }
        public int status { get; set; }
    }
    public class ModifyAction
    {
        public string action { get; set; }
        public long item_id { get; set; }
        public int? time { get; set; }
        public string tags { get; set; }
    }

    public enum Action
    {
        //    Basic Actions

        /// <summary>
        ///  add - Add a new item to the user's list (not supported)
        /// </summary>
        [StringValue("add")]
        Add = 0,
        /// <summary>
        /// archive - Move an item to the user's archive
        /// </summary>
        [StringValue("archive")]
        Archive = 1,
        /// <summary>
        /// readd - Re-add (unarchive) an item to the user's list
        /// </summary>
        [StringValue("readd")]
        Readd = 2,
        /// <summary>
        /// favorite - Mark an item as a favorite
        /// </summary>
        [StringValue("favorite")]
        Favorite = 3,
        /// <summary>
        /// unfavorite - Remove an item from the user's favorites
        /// </summary>
        [StringValue("unfavorite")]
        Unfavorite = 4,
        /// <summary>
        /// delete - Permanently remove an item from the user's account
        /// </summary>
        [StringValue("delete")]
        Delete = 5,

        //Tagging Actions
        /// <summary>
        /// tags_add - Add one or more tags to an item
        /// </summary>
        [StringValue("tags_add")]
        TagsAdd = 10,
        /// <summary>
        /// tags_remove - Remove one or more tags from an item
        /// </summary>
        [StringValue("tags_remove")]
        TagsRemove = 11,
        /// <summary>
        /// tags_replace - Replace all of the tags for an item with one or more provided tags
        /// </summary>
        [StringValue("tags_replace")]
        TagsReplace = 12,
        /// <summary>
        /// tags_clear - Remove all tags from an item
        /// </summary>
        [StringValue("tags_clear")]
        TagsClear = 13,
        /// <summary>
        /// tag_rename - Rename a tag; this affects all items with this tag (not supported)
        /// </summary>
        [StringValue("tag_rename")]
        TagRename = 14
    }


    [Serializable]
    public class PocketRestException : Exception
    {
        public PocketRestException() { }

        public PocketRestException(string message)
            : base(message) { }

        //public PocketRestException(string message, Exception inner) : base(message, inner)
        //{
        //}

        //protected PocketRestException(
        //    SerializationInfo info,
        //    StreamingContext context) : base(info, context)
        //{
        //}
    }

}
