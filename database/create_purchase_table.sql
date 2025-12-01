-- 建立 SurugayaPurchase 資料表
-- 用於記錄 Surugaya 商品的購買紀錄

CREATE TABLE IF NOT EXISTS public."SurugayaPurchase" (
    url TEXT PRIMARY KEY,
    date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    note TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 建立索引以提升查詢效能
CREATE INDEX IF NOT EXISTS idx_surugaya_purchase_date ON public."SurugayaPurchase"(date DESC);

-- 加入註解
COMMENT ON TABLE public."SurugayaPurchase" IS 'Surugaya 商品購買紀錄';
COMMENT ON COLUMN public."SurugayaPurchase".url IS '商品 URL (主鍵)';
COMMENT ON COLUMN public."SurugayaPurchase".date IS '購買日期';
COMMENT ON COLUMN public."SurugayaPurchase".note IS '備註';
COMMENT ON COLUMN public."SurugayaPurchase".created_at IS '建立時間';
