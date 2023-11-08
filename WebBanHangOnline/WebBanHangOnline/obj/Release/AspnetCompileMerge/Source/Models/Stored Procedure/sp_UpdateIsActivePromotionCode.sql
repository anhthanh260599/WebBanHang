CREATE PROCEDURE [dbo].[sp_UpdateIsActivePromotionCode]
AS
BEGIN
  SET NOCOUNT ON;
    DECLARE @CurrentTime DATETIME = GETDATE();
	DECLARE @TimeThreshold DATETIME;

    -- Thêm 14 giờ vào thời gian hiện tại
    SET @TimeThreshold = DATEADD(HOUR, 15, @CurrentTime);

	-- Xử lý cho trong khoảng thời gian
    UPDATE tb_Promotion
    SET IsActive = 1
    WHERE @TimeThreshold >= DateFrom
	AND @TimeThreshold <= DateTo;

	-- Xử lý cho ngoài khoảng thời gian
	UPDATE tb_Promotion
    SET IsActive = 0
    WHERE @TimeThreshold < DateFrom
	OR @TimeThreshold > DateTo;

	-- Xử lý cho trong khoảng thời gian và Quantity < 1
	UPDATE tb_Promotion
    SET IsActive = 0
    WHERE Quantity < 1
	AND(@TimeThreshold >= DateFrom
	AND @TimeThreshold <= DateTo);
END;