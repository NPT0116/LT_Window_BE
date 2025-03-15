using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using src.Dto.Invoice;
using src.Interfaces;
using SelectPdf;

namespace src.Services
{
    public class HtmlInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVariantRepository _variantRepository;
        private readonly IColorRepository _colorRepository;

        public HtmlInvoiceService(
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,
            IVariantRepository variantRepository,
            IColorRepository colorRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _variantRepository = variantRepository;
            _colorRepository = colorRepository;
        }

        /// <summary>
        /// Sinh HTML hóa đơn với ảnh (phiên bản gửi điện tử).
        /// </summary>
        public async Task<string> GenerateInvoiceHtmlAsync(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Không tìm thấy hóa đơn với ID: {invoiceId}");

            var customer = await _customerRepository.GetCustomerByIdAsync(invoice.CustomerID);

            float totalAmount = 0;
            var sbRows = new StringBuilder();
            int index = 1;

            foreach (var detail in invoice.InvoiceDetails)
            {
                var variantName = await _variantRepository.GetVariantNameByIdAsync(detail.VariantID);
                var variantImageUrl = await _colorRepository.GetImageByVariantIdAsync(detail.VariantID);

                var (imgBytes, contentType) = await DownloadImageWithContentTypeAsync(variantImageUrl);
                string imgTag = (imgBytes.Length > 0)
                    ? $"<img src='data:{contentType};base64,{Convert.ToBase64String(imgBytes)}' style='width:50px; height:auto;' />"
                    : "(No Image)";

                float lineTotal = detail.Price * detail.Quantity;
                totalAmount += lineTotal;

                sbRows.Append($@"
                    <tr>
                        <td>{index}</td>
                        <td>{imgTag}</td>
                        <td>{variantName}</td>
                        <td>{detail.Quantity}</td>
                        <td>{detail.Price:N0}</td>
                        <td>{lineTotal:N0}</td>
                    </tr>");
                index++;
            }

            string htmlTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: DejaVu Sans, sans-serif; }
        .invoice-container { width: 700px; margin: auto; padding: 20px; border: 1px solid #ccc; }
        .header { text-align: center; }
        .title { font-size: 24px; font-weight: bold; }
        table { width: 100%; border-collapse: collapse; margin-top: 10px; }
        table th, table td { border: 1px solid #ccc; padding: 8px; text-align: left; vertical-align: middle; }
        .summary { text-align: right; margin-top: 20px; }
        .footer-sign { margin-top: 40px; }
        .footer-sign div { width: 50%; float: left; text-align: center; }
        .clearfix { clear: both; }
    </style>
</head>
<body>
<div class='invoice-container'>
  <div class='header'>
    <div class='title'>HÓA ĐƠN MUA HÀNG</div>
    <div>Công ty Điện thoại Minh Tuấn</div>
    <div>Địa chỉ: 123 Lê Lợi, Q.1, TP.HCM</div>
    <div>Điện thoại: 0123 456 789</div>
  </div>
  <hr />
  <strong>Ngày lập:</strong> {DATE}<br/>
  <strong>Mã HĐ:</strong> {INVOICE_NUMBER}<br/>
  <strong>Khách hàng:</strong> {CUSTOMER_NAME}<br/>
  <strong>Email:</strong> {CUSTOMER_EMAIL}<br/>
  <strong>Điện thoại:</strong> {CUSTOMER_PHONE}<br/>
  <strong>Địa chỉ:</strong> {CUSTOMER_ADDRESS}<br/>
  <table>
    <thead>
      <tr>
        <th>STT</th>
        <th>Ảnh</th>
        <th>Sản phẩm</th>
        <th>SL</th>
        <th>Đơn giá</th>
        <th>Thành tiền</th>
      </tr>
    </thead>
    <tbody>
      {ITEM_ROWS}
    </tbody>
  </table>
  <div class='summary'>
    <strong>Tổng tiền: </strong> {TOTAL_AMOUNT} VNĐ
  </div>
  <div class='footer-sign'>
    <div>
      <strong>Người mua hàng</strong><br/><br/><br/><br/><br/>
      (Ký, ghi rõ họ tên)
    </div>
    <div>
      <strong>Người bán hàng</strong><br/><br/><br/><br/><br/>
      (Ký, ghi rõ họ tên)
    </div>
    <div class='clearfix'></div>
  </div>
</div>
</body>
</html>
";
            string htmlContent = htmlTemplate
                .Replace("{DATE}", invoice.Date.ToString("dd/MM/yyyy"))
                .Replace("{INVOICE_NUMBER}", invoice.InvoiceID.ToString())
                .Replace("{CUSTOMER_NAME}", customer.Name)
                .Replace("{CUSTOMER_EMAIL}", customer.Email ?? "")
                .Replace("{CUSTOMER_PHONE}", customer.Phone ?? "")
                .Replace("{CUSTOMER_ADDRESS}", customer.Address ?? "")
                .Replace("{ITEM_ROWS}", sbRows.ToString())
                .Replace("{TOTAL_AMOUNT}", totalAmount.ToString("N0"));

            return htmlContent;
        }

        /// <summary>
        /// Sinh HTML hóa đơn không có mục ảnh (phiên bản in).
        /// </summary>
        public async Task<string> GenerateInvoiceHtmlNoImageAsync(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Không tìm thấy hóa đơn với ID: {invoiceId}");

            var customer = await _customerRepository.GetCustomerByIdAsync(invoice.CustomerID);

            float totalAmount = 0;
            var sbRows = new StringBuilder();
            int index = 1;

            foreach (var detail in invoice.InvoiceDetails)
            {
                var variantName = await _variantRepository.GetVariantNameByIdAsync(detail.VariantID);

                float lineTotal = detail.Price * detail.Quantity;
                totalAmount += lineTotal;

                // Bỏ cột ảnh
                sbRows.Append($@"
                    <tr>
                        <td>{index}</td>
                        <td>{variantName}</td>
                        <td>{detail.Quantity}</td>
                        <td>{detail.Price:N0}</td>
                        <td>{lineTotal:N0}</td>
                    </tr>");
                index++;
            }

            // Template không có cột ảnh
            string htmlTemplate = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: DejaVu Sans, sans-serif; }
        .invoice-container { width: 700px; margin: auto; padding: 20px; border: 1px solid #ccc; }
        .header { text-align: center; }
        .title { font-size: 24px; font-weight: bold; }
        table { width: 100%; border-collapse: collapse; margin-top: 10px; }
        table th, table td { border: 1px solid #ccc; padding: 8px; text-align: left; vertical-align: middle; }
        .summary { text-align: right; margin-top: 20px; }
        .footer-sign { margin-top: 40px; }
        .footer-sign div { width: 50%; float: left; text-align: center; }
        .clearfix { clear: both; }
    </style>
</head>
<body>
<div class='invoice-container'>
  <div class='header'>
    <div class='title'>HÓA ĐƠN MUA HÀNG</div>
    <div>Công ty Điện thoại Minh Tuấn</div>
    <div>Địa chỉ: 123 Lê Lợi, Q.1, TP.HCM</div>
    <div>Điện thoại: 0123 456 789</div>
  </div>
  <hr />
  <strong>Ngày lập:</strong> {DATE}<br/>
  <strong>Mã HĐ:</strong> {INVOICE_NUMBER}<br/>
  <strong>Khách hàng:</strong> {CUSTOMER_NAME}<br/>
  <strong>Email:</strong> {CUSTOMER_EMAIL}<br/>
  <strong>Điện thoại:</strong> {CUSTOMER_PHONE}<br/>
  <strong>Địa chỉ:</strong> {CUSTOMER_ADDRESS}<br/>
  <table>
    <thead>
      <tr>
        <th>STT</th>
        <th>Sản phẩm</th>
        <th>SL</th>
        <th>Đơn giá</th>
        <th>Thành tiền</th>
      </tr>
    </thead>
    <tbody>
      {ITEM_ROWS}
    </tbody>
  </table>
  <div class='summary'>
    <strong>Tổng tiền: </strong> {TOTAL_AMOUNT} VNĐ
  </div>
  <div class='footer-sign'>
    <div>
      <strong>Người mua hàng</strong><br/><br/><br/><br/><br/>
      (Ký, ghi rõ họ tên)
    </div>
    <div>
      <strong>Người bán hàng</strong><br/><br/><br/><br/><br/>
      (Ký, ghi rõ họ tên)
    </div>
    <div class='clearfix'></div>
  </div>
</div>
</body>
</html>
";

            string htmlContent = htmlTemplate
                .Replace("{DATE}", invoice.Date.ToString("dd/MM/yyyy"))
                .Replace("{INVOICE_NUMBER}", invoice.InvoiceID.ToString())
                .Replace("{CUSTOMER_NAME}", customer.Name)
                .Replace("{CUSTOMER_EMAIL}", customer.Email ?? "")
                .Replace("{CUSTOMER_PHONE}", customer.Phone ?? "")
                .Replace("{CUSTOMER_ADDRESS}", customer.Address ?? "")
                .Replace("{ITEM_ROWS}", sbRows.ToString())
                .Replace("{TOTAL_AMOUNT}", totalAmount.ToString("N0"));

            return htmlContent;
        }

        /// <summary>
        /// Chuyển đổi HTML thành PDF sử dụng SelectPdf.
        /// </summary>
public byte[] ConvertHtmlToPdfUsingSelectPdf(string htmlContent)
{
    var converter = new HtmlToPdf();

    // Chờ 1 giây nếu cần
    System.Threading.Thread.Sleep(1000);

    converter.Options.PdfPageSize = PdfPageSize.A4;
    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
    converter.Options.MarginTop = 20;
    converter.Options.MarginBottom = 20;
    converter.Options.MarginLeft = 20;
    converter.Options.MarginRight = 20;

    var doc = converter.ConvertHtmlString(htmlContent);
    var pdfBytes = doc.Save();
    doc.Close();
    return pdfBytes;
}


        /// <summary>
        /// Tải ảnh và lấy content-type từ header, trả về (bytes, contentType). Nếu lỗi, trả về (mảng rỗng, "image/png").
        /// </summary>
        private async Task<(byte[] bytes, string contentType)> DownloadImageWithContentTypeAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return (Array.Empty<byte>(), "image/png");

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
                return (Array.Empty<byte>(), "image/png");

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/png";
            return (bytes, contentType);
        }

        /// <summary>
        /// Lớp chứa thông tin chi tiết cho mỗi dòng bảng.
        /// </summary>
        private class InvoiceDetailInfo
        {
            public int Index { get; set; }
            public string VariantName { get; set; }
            public int Quantity { get; set; }
            public float Price { get; set; }
            public float LineTotal { get; set; }
            public byte[] ImageBytes { get; set; }
        }
    }
}
