﻿using EcommerceBookApp.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public UnitOfWork(AppDbContext db)
        { 
            _db = db;
            Category = new CategoryRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            AppUser = new AppUserRepository(_db);
            ShopCart = new ShopCartRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }

        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShopCartRepository ShopCart { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
