using Microsoft.AspNetCore.Mvc;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Amazon.S3;

namespace SNSCakeBakery_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public ProductsController(IImageService imageService, IConfiguration config)
        {
            _imageService = imageService;
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Uploads an image to R2 and saves the metadata to Oracle.
        /// Fixes ORA-01400 by providing a default DISPLAYORDER.
        /// </summary>
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> UploadProductImage(
            int productId, 
            IFormFile file, 
            [FromForm] int imageTypeId, 
            [FromForm] bool isPrimary)
        {
            if (file == null || file.Length == 0) 
                return BadRequest("Please provide a valid image file.");

            string storageKey = string.Empty;

            try
            {
                // 1. Upload to Cloudflare R2 via our fixed ImageService
                storageKey = await _imageService.UploadImageAsync(file, $"products/{productId}");

                // 2. Insert into Oracle using Dapper (UPPERCASE SCHEMA)
                using (IDbConnection db = new OracleConnection(_connectionString))
                {
                    // Added DISPLAYORDER to satisfy the NOT NULL constraint (ORA-01400)
                    string sql = @"
                        INSERT INTO PRODUCTIMAGES (PRODUCTID, IMAGETYPEID, STORAGEKEY, ISPRIMARY, DISPLAYORDER)
                        VALUES (:ProductId, :ImageTypeId, :StorageKey, :IsPrimary, :DisplayOrder)";

                    var parameters = new
                    {
                        ProductId = productId,
                        ImageTypeId = imageTypeId,
                        StorageKey = storageKey,
                        IsPrimary = isPrimary ? 1 : 0, // Mapping bool to Oracle NUMBER(1)
                        DisplayOrder = 0               // Defaulting to 0 to prevent ORA-01400
                    };

                    await db.ExecuteAsync(sql, parameters);
                }

                return Ok(new { Status = "Success", Path = storageKey });
            }
            catch (Exception ex)
            {
                // 3. Cleanup: If Database fails, delete the orphan file from R2 to keep storage clean
                if (!string.IsNullOrEmpty(storageKey))
                {
                    await _imageService.DeleteImageAsync(storageKey);
                }

                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all active products with their primary image URL.
        /// </summary>
        [HttpGet]
       [HttpGet]
public async Task<IActionResult> GetProducts()
{
    using (IDbConnection db = new OracleConnection(_connectionString))
    {
        // Principal Tip: Use the section-based lookup for consistency
        var baseUrl = _config["CloudflareR2:PublicBaseUrl"]?.TrimEnd('/');

        string sql = $@"
            SELECT P.PRODUCTID, P.NAME, P.DESCRIPTION, P.BASEPRICE, PT.TYPENAME AS PRODUCTTYPENAME,
                   '{baseUrl}/' || PI.STORAGEKEY AS MAINIMAGEURL
            FROM PRODUCTS P
            JOIN PRODUCTTYPES PT ON P.PRODUCTTYPEID = PT.TYPEID
            LEFT JOIN PRODUCTIMAGES PI ON P.PRODUCTID = PI.PRODUCTID AND PI.ISPRIMARY = 1
            WHERE P.ISACTIVE = 1";

        var products = await db.QueryAsync(sql);
        return Ok(products);
    }
}
    }
}