-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 127.0.0.1
-- Thời gian đã tạo: Th5 11, 2025 lúc 01:47 PM
-- Phiên bản máy phục vụ: 10.4.32-MariaDB
-- Phiên bản PHP: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `personalfinancedb`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `budgets`
--

CREATE TABLE `budgets` (
  `Id` int(11) NOT NULL,
  `Category` longtext NOT NULL,
  `Amount` decimal(65,30) NOT NULL,
  `Period` longtext NOT NULL,
  `StartDate` datetime(6) NOT NULL,
  `EndDate` datetime(6) NOT NULL,
  `Description` longtext NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `UserId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `budgets`
--

INSERT INTO `budgets` (`Id`, `Category`, `Amount`, `Period`, `StartDate`, `EndDate`, `Description`, `CreatedAt`, `UpdatedAt`, `UserId`) VALUES
(2, 'Food', 5000000.000000000000000000000000000000, 'Daily', '2025-05-01 00:00:00.000000', '2025-05-31 00:00:00.000000', 'Ăn uống', '2025-05-11 13:38:52.606039', NULL, 9),
(3, 'Shopping', 100000.000000000000000000000000000000, 'Weekly', '2025-05-01 00:00:00.000000', '2025-05-31 00:00:00.000000', 'Mua sắm', '2025-05-11 13:39:21.929352', NULL, 9);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `transactions`
--

CREATE TABLE `transactions` (
  `Id` int(11) NOT NULL,
  `Amount` decimal(65,30) NOT NULL,
  `Type` longtext NOT NULL,
  `Category` longtext NOT NULL,
  `Description` longtext NOT NULL,
  `TransactionDate` datetime(6) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `UserId` int(11) NOT NULL,
  `WalletId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `transactions`
--

INSERT INTO `transactions` (`Id`, `Amount`, `Type`, `Category`, `Description`, `TransactionDate`, `CreatedAt`, `UpdatedAt`, `UserId`, `WalletId`) VALUES
(2, 2500000.000000000000000000000000000000, 'Expense', 'Food', 'Đi ăn KFC', '2025-05-14 00:00:00.000000', '2025-05-11 13:39:56.618815', '2025-05-11 17:25:12.564895', 9, 4),
(3, 500000.000000000000000000000000000000, 'Income', 'Lương', 'Lương tháng 5', '2025-05-05 00:00:00.000000', '2025-05-11 13:42:03.034215', NULL, 9, 3);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Email` longtext NOT NULL,
  `PasswordHash` longtext NOT NULL,
  `FirstName` longtext NOT NULL,
  `LastName` longtext NOT NULL,
  `PhoneNumber` longtext NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `users`
--

INSERT INTO `users` (`Id`, `Email`, `PasswordHash`, `FirstName`, `LastName`, `PhoneNumber`, `CreatedAt`, `UpdatedAt`, `IsActive`) VALUES
(9, 'huyln1302021@gmail.com', '$2a$11$pjiqxkHN0l673KE8T52.PO1k9gFgmQ8/j/WnCXTlqI7ryw40IjNQW', 'Huy', 'Lê Ngọc', 'huyln1302021@gmail.com', '2025-05-11 13:30:16.871569', '2025-05-11 17:07:18.616938', 1),
(10, 'huyln130202@gmail.com', '$2a$11$9U31nPA8NVnksvSrYIHVx.xAYJGy/ueDRRbTZ2Np/qu/LT01EJx7a', 'Huy', 'Lê Ngọc', '0905196052', '2025-05-11 13:31:09.420615', '2025-05-11 13:33:20.794288', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wallets`
--

CREATE TABLE `wallets` (
  `Id` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `Balance` decimal(65,30) NOT NULL,
  `Type` longtext NOT NULL,
  `Description` longtext NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `UserId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `wallets`
--

INSERT INTO `wallets` (`Id`, `Name`, `Balance`, `Type`, `Description`, `CreatedAt`, `UpdatedAt`, `IsActive`, `UserId`) VALUES
(2, 'Agribank', 5000000.000000000000000000000000000000, 'Bank', 'Tài khoản ngân hàng', '2025-05-11 13:37:29.361750', NULL, 1, 9),
(3, 'BIDV', 2500000.000000000000000000000000000000, 'Bank', 'Ngân hàng', '2025-05-11 13:37:46.078724', NULL, 1, 9),
(4, 'Momo', 7500000.000000000000000000000000000000, 'E-wallet', 'Ví điện tử', '2025-05-11 13:38:06.450012', NULL, 1, 9);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250407143634_InitialCreate', '8.0.2');

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `budgets`
--
ALTER TABLE `budgets`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Budgets_UserId` (`UserId`);

--
-- Chỉ mục cho bảng `transactions`
--
ALTER TABLE `transactions`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Transactions_UserId` (`UserId`),
  ADD KEY `IX_Transactions_WalletId` (`WalletId`);

--
-- Chỉ mục cho bảng `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- Chỉ mục cho bảng `wallets`
--
ALTER TABLE `wallets`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Wallets_UserId` (`UserId`);

--
-- Chỉ mục cho bảng `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT cho các bảng đã đổ
--

--
-- AUTO_INCREMENT cho bảng `budgets`
--
ALTER TABLE `budgets`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT cho bảng `transactions`
--
ALTER TABLE `transactions`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT cho bảng `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `wallets`
--
ALTER TABLE `wallets`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `budgets`
--
ALTER TABLE `budgets`
  ADD CONSTRAINT `FK_Budgets_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;

--
-- Các ràng buộc cho bảng `transactions`
--
ALTER TABLE `transactions`
  ADD CONSTRAINT `FK_Transactions_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Transactions_Wallets_WalletId` FOREIGN KEY (`WalletId`) REFERENCES `wallets` (`Id`) ON DELETE CASCADE;

--
-- Các ràng buộc cho bảng `wallets`
--
ALTER TABLE `wallets`
  ADD CONSTRAINT `FK_Wallets_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
