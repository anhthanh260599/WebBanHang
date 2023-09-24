CREATE PROCEDURE [dbo].[UpdateStatusIfNoChange] -- Stored Procedure này dùng để cập nhật trạng thái cho thanh toán Online
AS                                              -- Chuyển trạng thái từ "Chưa thanh toán" sang "Huỷ" nếu trong vòng 15p không thanh toán
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentTime DATETIME = GETDATE();
    DECLARE @TimeThreshold DATETIME;

    -- Thêm 14 giờ vào thời gian hiện tại
    SET @TimeThreshold = DATEADD(HOUR, 14, @CurrentTime);

    UPDATE tb_Order
    SET Status = 4
    WHERE TypePayment = 2
    AND Status = 1
    AND DATEDIFF(MINUTE, ModifierDate, @TimeThreshold) >= 15;
END;