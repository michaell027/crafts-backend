CREATE TABLE IF NOT EXISTS `users` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `username` varchar(255) NOT NULL,
    `password` varchar(255) NOT NULL,
    `email` varchar(255) NOT NULL,
    `created_at` datetime NOT NULL,
    `updated_at` datetime NOT NULL,
    PRIMARY KEY (`id`)
    ) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

INSERT INTO `users` (`username`, `password`, `email`, `created_at`, `updated_at`) VALUES
    ('admin', 'admin', 'admin@admin.sk', '2013-11-20 00:00:00', '2013-11-20 00:00:00');

INSERT INTO `users` (`username`, `password`, `email`, `created_at`, `updated_at`) VALUES
    ('john_doe', 'password123', 'john.doe@example.com', '2024-02-11 12:30:00', '2024-02-11 12:30:00');

INSERT INTO `users` (`username`, `password`, `email`, `created_at`, `updated_at`) VALUES
    ('jane_smith', 'securepass', 'jane.smith@example.com', '2024-02-11 13:45:00', '2024-02-11 13:45:00');

INSERT INTO `users` (`username`, `password`, `email`, `created_at`, `updated_at`) VALUES
    ('user123', 'userpass', 'user123@example.com', '2024-02-11 15:00:00', '2024-02-11 15:00:00');
