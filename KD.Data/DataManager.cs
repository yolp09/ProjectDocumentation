using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KD.Data
{
    public class DataManager
    {
        private static readonly KDv1Context _context = new KDv1Context();

        #region Запросы к Detail

        public void ChangeDetailIsArchive(int idDetail)
        {
            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            detail.Archive = !detail.Archive;

            _context.SaveChanges();
        }

        public void DeleteDetail(int idDetail)
        {
            List<Address> adr = _context.Addresses.Where(a => a.IdDetail == idDetail).ToList();
            foreach (var item in adr)
            {
                _context.Addresses.Remove(item);
            }

            List<Applicability> appl = _context.Applicabilities.Where(a => a.IdDetail == idDetail).ToList();
            foreach (var item in appl)
            {
                _context.Applicabilities.Remove(item);
            }

            List<ApplicabilitySB> applSB = _context.ApplicabilitySB.Where(a => a.idDetail == idDetail || a.IdDetailSB == idDetail).ToList();
            foreach (var item in applSB)
            {
                _context.ApplicabilitySB.Remove(item);
            }

            List<ApplicabilityNotice> applNotice = _context.ApplicabilityNotices.Where(a => a.IdDetail == idDetail).ToList();
            foreach (var item in applNotice)
            {
                _context.ApplicabilityNotices.Remove(item);
            }

            List<ApplicabilityFile> applFile = _context.ApplicabilityFiles.Where(a => a.IdDetail == idDetail).ToList();
            foreach (var item in applFile)
            {
                _context.ApplicabilityFiles.Remove(item);
            }

            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            _context.Details.Remove(detail);

            _context.SaveChanges();
        }
        public void ChangeIdFileDetail(int idDetail, int idFile)
        {
            _context.Details.Where(d => d.Id == idDetail).FirstOrDefault().IdFile = idFile;

            _context.SaveChanges();
        }
        public void ChangeArchiveDetail(int idDetail, string numberNotice)
        {
            _context.Details.Where(d => d.Id == idDetail).FirstOrDefault().Archive = true;
            _context.Details.Where(d => d.Id == idDetail).FirstOrDefault().ArchiveNotice = "извещением №" + numberNotice;

            _context.SaveChanges();
        }
        public void ChangeDetail(int idDetail, string number, string name, string developer, int? idFile)
        {
            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            if (_context.Details.Where(d => d.Number == number).Count() > 0 && detail.Number != number) throw new Exception("Деталь с таким обозначением уже существует!");
            detail.Number = number;
            detail.Name = name;
            detail.Developer = developer;
            if (idFile != null)
            {
                detail.IdFile = idFile;
                List<ApplicabilityFile> applicabilityFiles = _context.ApplicabilityFiles.Where(af => af.IdDetail == idDetail).ToList();
                applicabilityFiles.ElementAt(applicabilityFiles.Count - 1).IdFile = idFile.Value;
            }

            _context.SaveChanges();
        }
        public Detail ReturnDetail(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail);
        }
        public void AddDetail(int typeDetail, int idProduct, string name, string number, string developer, bool archive, int? idFile, int selectedIdSection)
        {
            Detail detail = new Detail { Number = number, Name = name, Developer = developer, TypeDetail = typeDetail, Archive = archive, IdFile = idFile };
            if (_context.Details.Any(d => d.Number == number)) throw new Exception("Деталь с таким обозначением уже существует!");

            _context.Details.Add(detail);
            if (selectedIdSection == 0)
            {
                Applicability applicability = new Applicability { IdProduct = idProduct, Detail = detail };
                _context.Applicabilities.Add(applicability);
            }
            else
            {
                ApplicabilitySB applicability = new ApplicabilitySB { IdDetailSB = idProduct, Detail = detail };
                _context.ApplicabilitySB.Add(applicability);
            }
            if (idFile != null) { AddApplicabiltyFile(detail, idFile.Value, "ver 0", "Начальное"); }

            _context.SaveChanges();
        }
        public void AddStandardDetail(int typeDetail, string name, bool archive, int? idFile)
        {
            Detail detail = new Detail { Name = name, TypeDetail = typeDetail, Archive = archive, IdFile = idFile };
            if (_context.Details.Any(d => d.Name == name)) throw new Exception("Изделие с таким наименованием уже существует!");
            _context.Details.Add(detail);
            if (idFile != null) { AddApplicabiltyFile(detail, idFile.Value, "ver 0", "Начальное"); }

            _context.SaveChanges();
        }
        public Task<List<Detail>> GetDetailsAsync(int typeDetail)
        {
            return Task.Run(() => _context.Details.Where(d => d.TypeDetail == typeDetail).ToList());
        }
        public List<Detail> GetDetails(int typeDetail)
        {
            return _context.Details.Where(d => d.TypeDetail == typeDetail).ToList();
        }
        public Detail GetDetailById(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail);
        }
        public List<int> GetIdDetails(int typeDetail)
        {
            if (typeDetail == 9)
                return _context.Details.Where(d => (d.Archive == true && d.TypeDetail != 4 && d.TypeDetail != 5 && d.TypeDetail != 8)).Select(d => d.Id).ToList();
            else
                return _context.Details.Where(d => d.TypeDetail == typeDetail).Select(d => d.Id).ToList();
        }

        public List<int> GetIdDetailsArchive()
        {
            return _context.Details.Where(d => d.ArchiveNotice.Length > 0).Select(d => d.Id).ToList();
        }

        public Task<List<int>> GetIdDetailsAsync(int typeDetail)
        {
            return Task.Run(() => GetIdDetails(typeDetail));
        }

        #endregion

        #region Запросы к Product

        public void AddProduct(string number, string name)
        {
            Product product = new Product { Number = number, Name = name };
            if (_context.Products.Any(p => p.Number == number)) throw new Exception("Изделие с таким обозначением уже существует!");
            _context.Products.Add(product);

            _context.SaveChanges();
        }
        public Task<List<Product>> GetProductsAsync()
        {
            return Task.Run(() => _context.Products.ToList());
        }
        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }
        public Product GetProductById(int idProduct)
        {
            return _context.Products.SingleOrDefault(p => p.Id == idProduct);
        }

        #endregion

        #region Запрос к Notice

        public List<int> GetIdNotices()
        {
            return _context.Notices.Select(d => d.Id).ToList();
        }
        public Task<List<int>> GetIdNoticesAsync()
        {
            return Task.Run(() => GetIdNotices());
        }
        public void DeleteNotice(int idNotice)
        {
            List<AddressNotice> adr = _context.AddressNotices.Where(a => a.IdNotice == idNotice).ToList();
            foreach (var item in adr)
            {
                _context.AddressNotices.Remove(item);
            }

            List<ApplicabilityNotice> aapl = _context.ApplicabilityNotices.Where(a => a.IdNotice == idNotice).ToList();
            foreach (var item in aapl)
            {
                _context.ApplicabilityNotices.Remove(item);
            }

            Notice notice = _context.Notices.SingleOrDefault(n => n.Id == idNotice);
            _context.Notices.Remove(notice);

            _context.SaveChanges();
        }
        public void ChangeNotice(int idNotice, string number, DateTime date, string developer, int countSheets, string changeCode, int? idFile)
        {
            Notice notice = _context.Notices.SingleOrDefault(n => n.Id == idNotice);

            if (_context.Notices.Where(n => n.Number == number).Count() > 0 && notice.Number != number) throw new Exception("Извещение с таким номером уже существует!");
            notice.Number = number;
            notice.Date = date;
            notice.Developer = developer;
            notice.CountSheets = countSheets;
            notice.ChangeCode = changeCode;
            if (idFile != null)
            {
                notice.IdFile = idFile.Value;
            }

            _context.SaveChanges();
        }
        public void AddNotice(string number, DateTime date, string developer, int countSheets, string changeCode, int idFile, List<Tuple<Detail, string>> noticeDetails)
        {
            Notice notice = new Notice { Number = number, Date = date, Developer = developer, CountSheets = countSheets, ChangeCode = changeCode, IdFile = idFile };
            if (_context.Notices.Any(n => n.Number == number)) throw new Exception("Извещение с таким номером уже существует!");
            _context.Notices.Add(notice);

            AddApplicabilityNotice(notice.Id, noticeDetails);

            _context.SaveChanges();
        }
        public List<Notice> GetNotices()
        {
            return _context.Notices.ToList();
        }
        public Task<List<Notice>> GetNoticesAsync()
        {
            return Task.Run(() => GetNotices());
        }
        public Notice GetNoticeById(int idNotice)
        {
            return _context.Notices.SingleOrDefault(n => n.Id == idNotice);
        }

        #endregion

        #region Запос к FileTable

        public List<MFile> GetUnusedFiles()
        {
            List<MFile> query = _context.FileTables.Select(ft => new MFile() { Id = ft.Id, FileName = ft.FileName }).ToList();
            List<MFile> result = _context.FileTables.Where(ft => (ft.ApplicabilityFiles.Count == 0 && ft.Details.Count == 0 && ft.Notices.Count == 0)).Select(f => new MFile() { Id = f.Id, FileName = f.FileName }).ToList();

            

            return result;
        }
        public void DeleteFile(int idFile)
        {
            FileTable fileTable = _context.FileTables.SingleOrDefault(f => f.Id == idFile);
            _context.FileTables.Remove(fileTable);

            _context.SaveChanges();
        }
        public int AddFile(string name, byte[] fileData)
        {
            FileTable file = new FileTable { FileName = name, FileData = fileData };
            _context.FileTables.Add(file);
            _context.SaveChanges();

            return file.Id;
        }
        public byte[] DataFile(int idFile, ref string fileName)
        {
            FileTable file = _context.FileTables.SingleOrDefault(ft => ft.Id == idFile);
            string path = System.IO.Path.GetTempPath();
            fileName = path + file.FileName;
            if (!File.Exists(fileName))
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.Write(file.FileData, 0, file.FileData.Length);
                }
            }
            return file.FileData;
        }

        #endregion

        //Таблица применяемости Детали и Изделия
        #region Запрос к Applicability

        public void DeleteApplicability(int idApplicability)
        {
            Applicability applicability = _context.Applicabilities.SingleOrDefault(a => a.Id == idApplicability);
            _context.Applicabilities.Remove(applicability);

            _context.SaveChanges();
        }
        public List<Applicability> GetaApplicabilityProduct(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail).Applicabilities.ToList();
        }
        public void AddApplicability(int idDetail, int idProduct, int selectedIdSection)
        {
            if (selectedIdSection == 0)
            {
                Applicability applicability = new Applicability { IdProduct = idProduct, IdDetail = idDetail };
                _context.Applicabilities.Add(applicability);
            }
            else
            {
                ApplicabilitySB applicability = new ApplicabilitySB { IdDetailSB = idProduct, idDetail = idDetail };
                _context.ApplicabilitySB.Add(applicability);
            }

            _context.SaveChanges();
        }
        public List<Detail> GetApplicabilityDetails(int idProduct)
        {
            List<Detail> result = new List<Detail>();

            Product product = _context.Products.SingleOrDefault(p => p.Id == idProduct);
            foreach (var applicability in product.Applicabilities.OrderBy(a => a.Detail.Number)) { result.Add(applicability.Detail); }

            return result;
        }

        public Task<List<Detail>> GetApplicabilityDetailsAsync(int idProdcut)
        {
            return Task.Run(() => GetApplicabilityDetails(idProdcut));
        }
        public List<Product> GetApplicabilityProducts(int idDetail)
        {
            List<Product> result = new List<Product>();

            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            foreach (var applicability in detail.Applicabilities) { result.Add(applicability.Product); }

            return result;
        }

        #endregion

        //Таблица применяемости Детали и Детали
        #region Запрос к ApplicabilitySB

        public void DeleteApplicabilitySB(int idApplicability)
        {
            ApplicabilitySB applicability = _context.ApplicabilitySB.SingleOrDefault(a => a.Id == idApplicability);
            _context.ApplicabilitySB.Remove(applicability);

            _context.SaveChanges();
        }
        public List<ApplicabilitySB> GetaApplicabilitySB(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail).ApplicabilitySBDetails.ToList();
        }
        //Поиск всех СБ, в которые входит деталь
        public List<Detail> GetApplicabilityDetailSB(int idDetail)
        {
            List<Detail> result = new List<Detail>();

            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            foreach (var applicability in detail.ApplicabilitySBDetails) { result.Add(applicability.DetailSB); }

            return result;
        }
        //Поиск всех деталей входящих в СБ
        public List<Detail> GetApplicabilitySBDetails(int idDetail)
        {
            List<Detail> result = new List<Detail>();

            Detail detail = _context.Details.SingleOrDefault(d => d.Id == idDetail);
            foreach (var applicability in detail.ApplicabilitySBProducts.OrderBy(a => a.Detail.Number)) { result.Add(applicability.Detail); }

            return result;
        }

        public Task<List<Detail>> GetApplicabilitySBDetailsAsync(int idDetail)
        {
            return Task.Run(() => GetApplicabilitySBDetails(idDetail));
        }

        #endregion

        #region Запрос к ApplicabilityFile

        public List<ApplicabilityFile> GetApplicabilityFiles(int idDetail)
        {
            return _context.ApplicabilityFiles.Where(af => af.IdDetail == idDetail).ToList();
        }
        public void AddApplicabiltyFile(Detail detail, int idFile, string version, string numberNotice)
        {
            ApplicabilityFile applicabilityFile = new ApplicabilityFile { Detail = detail, IdFile = idFile, Version = version, Notice = numberNotice };
            _context.ApplicabilityFiles.Add(applicabilityFile);

            _context.SaveChanges();
        }
        public int CountApplicabilityFile(int idDetail)
        {
            return _context.ApplicabilityFiles.Where(af => af.IdDetail == idDetail).Count();
        }

        #endregion

        #region Запрос к ApplicabilityNotice

        public void DeleteApplicabilityNotice(int idDetail, int idNotice)
        {
            ApplicabilityNotice item = _context.ApplicabilityNotices.SingleOrDefault(apn => (apn.IdDetail == idDetail && apn.IdNotice == idNotice));
            _context.ApplicabilityNotices.Remove(item);

            _context.SaveChanges();
        }
        public List<ApplicabilityNotice> GetApplicabilityNotice(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail).ApplicabilityNotices.ToList();
        }

        public List<int> GetApplicabilityNoticeId(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail).ApplicabilityNotices.Select(an => an.Notice.Id).ToList();
        }

        public List<ApplicabilityNotice> GetApplicabilityNoticeByIdNotice(int idNotice)
        {
            return _context.Notices.SingleOrDefault(n => n.Id == idNotice).ApplicabilityNotices.ToList();
        }
        public void AddApplicabilityNotice(int idNotice, List<Tuple<Detail, string>> details)
        {
            ApplicabilityNotice applicabilityNotice;
            foreach (var detail in details)
            {
                applicabilityNotice = new ApplicabilityNotice { IdNotice = idNotice, Detail = detail.Item1, Description = detail.Item2 };
                _context.ApplicabilityNotices.Add(applicabilityNotice);
            }

            _context.SaveChanges();
        }

        #endregion

        #region Запрос к таблицам с Adress

        public void AddAddress(int idDetail, string address, bool subscription)
        {
            Address item = new Address { IdDetail = idDetail, Address1 = address, Subscription = subscription };
            _context.Addresses.Add(item);

            _context.SaveChanges();
        }
        public void AddAddressNotice(int idNotice, string address, bool subscription)
        {
            AddressNotice item = new AddressNotice { IdNotice = idNotice, Address1 = address, Subscription = subscription };
            _context.AddressNotices.Add(item);

            _context.SaveChanges();
        }
        public List<Address> GetAdress(int idDetail)
        {
            return _context.Details.SingleOrDefault(d => d.Id == idDetail).Addresses.ToList();
        }
        public List<AddressNotice> GetAdressNotices(int idNotice)
        {
            return _context.Notices.SingleOrDefault(n => n.Id == idNotice).AddressNotices.ToList();
        }

        public void DeleteAddress(int idAddress)
        {
            Address address = _context.Addresses.SingleOrDefault(a => a.Id == idAddress);
            _context.Addresses.Remove(address);

            _context.SaveChanges();
        }
        public void DeleteAddressNotice(int idAddress)
        {
            AddressNotice address = _context.AddressNotices.SingleOrDefault(a => a.Id == idAddress);
            _context.AddressNotices.Remove(address);

            _context.SaveChanges();
        }

        #endregion

        #region Запрос к User

        public int CheckUser(string userName, string userPassword)
        {
            User user = _context.Users.SingleOrDefault(u => u.UserName == userName && u.UserPassword == userPassword);
            if (user == null) throw new Exception("Не верно указаны логин или пароль. Повторите ввод.");
            return user.AccessLevel;
        }

        #endregion

        #region Запрос к History

        public void SaveHistory(string date, string userName)
        {
            History history = new History { Date = date, UserName = userName };

            _context.History.Add(history);
            _context.SaveChangesAsync();
        }

        #endregion
    }
}
