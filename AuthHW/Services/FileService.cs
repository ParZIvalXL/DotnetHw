using AuthHW.Configuration;
using Microsoft.Extensions.Options;

namespace AuthHW.Services;

using Minio;
using Minio.DataModel.Args;

public class FileService
{
    private readonly MinioClient _minio;
    private readonly FileStorageOptions _options;

    public FileService(MinioClient minio, IOptions<FileStorageOptions> options)
    {
        _minio = minio;
        _options = options.Value;
    }

    public async Task EnsureBucketAsync()
    {
        var exists = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket)
        );

        if (!exists)
        {
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.Bucket)
            );
        }
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType
    )
    {
        await EnsureBucketAsync();

        var objectName = $"{Guid.NewGuid()}_{fileName}";

        await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_options.Bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType)
        );

        return $"{_options.Endpoint}/{_options.Bucket}/{objectName}";
    }
}
