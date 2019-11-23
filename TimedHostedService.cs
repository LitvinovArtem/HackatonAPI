using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebDekAPI;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using WebDekAPI.Controllers.Portfolio;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebDekAPI.Share;
using static WebDekAPI.Service;
using System.IO;
using System.Xml.Serialization;

internal class TimedHostedService : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private Timer _timer;
    private DataBaseContext db;
    private IMemoryCache _cache;




    public TimedHostedService(ILogger<TimedHostedService> logger, DataBaseContext context, IMemoryCache memoryCache)
    {
        _logger = logger;
        db = context;
        _cache = memoryCache;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory("files\\RatingCache\\");
        string[] files = Directory.GetFiles("files\\RatingCache\\");
        foreach (string file in files)
        {
            // TODO: Дописать очистку мусора (старых файлов)

            var tmp = LoadFromFile(file);
            string cacheName = "RatingLists" + tmp.categoryID.ToString() + tmp.year + tmp.sem.ToString();
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(24));
            _cache.Set(cacheName, tmp, cacheEntryOptions);

        }

        _logger.LogInformation("Timed Background Service is starting.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }
    private RatingLists LoadFromFile(string filePath)
    {
        RatingLists ratingLists = new RatingLists();
        XmlSerializer formatter = new XmlSerializer(ratingLists.GetType());
        FileStream aFile = new FileStream(filePath, FileMode.Open);
        byte[] buffer = new byte[aFile.Length];
        aFile.Read(buffer, 0, (int)aFile.Length);
        MemoryStream stream = new MemoryStream(buffer);
        return (RatingLists)formatter.Deserialize(stream);
    }
    private void SaveToFile(RatingLists ratingLists, string fileName)
    {
        string path = "files\\RatingCache\\" + fileName;
        FileStream outFile = File.Create(path);
        XmlSerializer formatter = new XmlSerializer(ratingLists.GetType());
        formatter.Serialize(outFile, ratingLists);
        outFile.Close();
    }
    private void DoWork(object state)
    {
        _logger.LogInformation("Timed Background Service is working.");

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Background Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}