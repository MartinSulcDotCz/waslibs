﻿using AppStudio.DataProviders.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AppStudio.DataProviders.WordPress
{
    public class WordPressParser : IParser<WordPressSchema>, IPaginationParser<WordPressSchema>
    {
        public IEnumerable<WordPressSchema> Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var wordPressItems = JsonConvert.DeserializeObject<WordPressResponse>(data);

            return (from r in wordPressItems.posts
                    select new WordPressSchema()
                    {
                        _id = r.id,
                        Title = r.title.DecodeHtml(),
                        Summary = r.excerpt.DecodeHtml(),
                        Content = r.content,
                        Author = r.author.name.DecodeHtml(),
                        ImageUrl = r.featured_image,
                        PublishDate = r.date,
                        FeedUrl = r.url
                    });
        }

        IResponseBase<WordPressSchema> IPaginationParser<WordPressSchema>.Parse(string data)
        {
            var result = new GenericResponse<WordPressSchema>();
            if (string.IsNullOrEmpty(data))
            {
                return result;
            }

            var wordPressItems = JsonConvert.DeserializeObject<WordPressResponse>(data);

            var items = (from r in wordPressItems.posts
                         select new WordPressSchema()
                         {
                             _id = r.id,
                             Title = r.title.DecodeHtml(),
                             Summary = r.excerpt.DecodeHtml(),
                             Content = r.content,
                             Author = r.author.name.DecodeHtml(),
                             ImageUrl = r.featured_image,
                             PublishDate = r.date,
                             FeedUrl = r.url
                         });

            foreach (var item in items)
            {
                result.Add(item);               
            }

            result.NextPageToken = wordPressItems?.meta?.next_page;
            return result;
        }
    }
}
