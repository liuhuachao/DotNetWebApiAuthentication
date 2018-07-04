﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DDD.Application.Interfaces;
using DDD.Application.Dtos;
using DDD.Domain.Interfaces;

namespace DDD.Application.Services
{
    /// <summary>
    /// 首页应用服务
    /// </summary>
    public class HomeService : IHomeAppService
    {
        private readonly ICacheService _cacheSevice;
        private readonly IHomesRepository _repository;
        private readonly ICmsContentsRepository _newsRepository;
        private readonly IVideosRepository _videoRepository;

        public HomeService(ICacheService cacheSevice, IHomesRepository repository,ICmsContentsRepository newsRepository,IVideosRepository videoRepository)
        {
            this._cacheSevice = cacheSevice;
            this._repository = repository;
            this._newsRepository = newsRepository;
            this._videoRepository = videoRepository;
        }

        public IList<HomeList> GetList(int pageIndex = 1, int pageSize = 5)
        {
            IList<HomeList> homeList;
            string cacheKey = string.Format("home_list_{0}_{1}",pageIndex,pageSize);
            if (!this._cacheSevice.Exists(cacheKey))
            {
                homeList = this._repository.GetList(pageIndex, pageSize);
                if (homeList != null)
                {
                    this._cacheSevice.Set(cacheKey, homeList);
                }
            }
            else
            {
                homeList = this._cacheSevice.Get<List<HomeList>>(cacheKey);
            }                      
            return homeList;
        }

        public HomeDetail GetDetail(int id, int type)
        {
            HomeDetail homeDetail;
            string cacheKey = string.Format("home_detail_{0}_{1}", id, type);
            homeDetail = this._cacheSevice.Get<HomeDetail>(cacheKey);
            if (homeDetail == null)
            {
                homeDetail = this._repository.GetDetail(id, type);
                if(homeDetail != null)
                {
                    this._cacheSevice.Set(cacheKey, homeDetail);
                }                
            }
            return homeDetail;
        }

        public IList<HomeList> GetMore(int id, int type)
        {
            IList<HomeList> homeList;
            string cacheKey = string.Format("home_more_{0}_{1}", id, type);
            homeList = this._cacheSevice.Get<IList<HomeList>>(cacheKey);
            if (homeList == null)
            {
                homeList = this._repository.GetMore(id, type);
                if(homeList != null)
                {
                    this._cacheSevice.Set(cacheKey, homeList, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
                }                
            }
            return homeList;
        }

        public IList<HomeSearch> Search(string title)
        {
            IList<HomeSearch> homeSearch;
            string cacheKey = string.Format("home_search_{0}", title);
            homeSearch = this._cacheSevice.Get<List<HomeSearch>>(cacheKey);
            if (homeSearch == null)
            {
                homeSearch = this._repository.Search(title);
                if (homeSearch != null)
                {
                    this._cacheSevice.Set(cacheKey, homeSearch, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
                }                
            }
            return homeSearch;
        }

        public IList<HotSearch> HotSearch(int limit = 8)
        {
            IList<HotSearch> hotSearch;
            string cacheKey = string.Format("hot_search_{0}", limit);
            hotSearch = this._cacheSevice.Get<List<HotSearch>>(cacheKey);
            if (hotSearch == null)
            {
                hotSearch = this._repository.HotSearch(limit);
                if (hotSearch != null)
                {
                    this._cacheSevice.Set(cacheKey, hotSearch, TimeSpan.FromDays(1), TimeSpan.FromDays(1));
                }                
            }
            return hotSearch;
        }

        public bool ExistCache(string cacheKey)
        {
            return this._cacheSevice.Exists(cacheKey);
        }

        public bool RemoveCache(int id,int type)
        {           
            for (int i = 2; i <= 20; i++)
            {
                string cacheKey3 = string.Format("home_list_{0}_{1}", i, 10);
                if (ExistCache(cacheKey3))
                {
                    this._cacheSevice.Remove(cacheKey3);
                }
            }
            return ReplaceCache(id,type);
        }

        public bool ReplaceCache(int id, int type)
        {
            string cacheKey1 = string.Format("home_detail_{0}_{1}", id, type);
            string cacheKey2 = string.Format("{0}_detail_{1}", (type == 3) ? "video" : "news", id);
            string cacheKey3 = string.Format("home_list_{0}_{1}", 1, 5);

            if (ExistCache(cacheKey1))
            {
                if(this._cacheSevice.Remove(cacheKey1))
                {
                    var homeDetail = this._repository.GetDetail(id, type);
                    if (homeDetail != null)
                    {
                        this._cacheSevice.Set(cacheKey1, homeDetail);
                    }
                }
            }
            if (ExistCache(cacheKey2))
            {
                if (this._cacheSevice.Remove(cacheKey2))
                {
                    if (type == 3)
                    {
                        var video = this._videoRepository.GetDetail(id);
                        if (video != null)
                            this._cacheSevice.Set(cacheKey2, video);
                    }
                    else
                    {
                        var news = this._newsRepository.GetDetail(id);
                        if (news != null)
                            this._cacheSevice.Set(cacheKey2, news);

                    }
                }
            }
            if (ExistCache(cacheKey3))
            {
                if (this._cacheSevice.Remove(cacheKey3))
                {
                    var homeList = this._repository.GetList(1, 5);
                    if (homeList != null)
                    {
                        this._cacheSevice.Set(cacheKey3, homeList);
                    }
                }
            }
            return !ExistCache(cacheKey1) && !ExistCache(cacheKey2) && !ExistCache(cacheKey3);
        }
    }
}
