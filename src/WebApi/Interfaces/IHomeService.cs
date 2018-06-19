﻿using System.Collections.Generic;
using WebApi.Dtos;

namespace WebApi.Services
{
    public interface IHomeService
    {
        HomeDetail GetDetail(int id, int type);
        IList<HomeList> GetList(int pageIndex = 1, int pageSize = 8);
        IList<HotSearch> HotSearch(int limit = 10);
        IList<HomeSearch> Search(string title);
    }
}