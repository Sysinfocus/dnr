-- sqlite Data Source=.\app.db;Cache=Shared;

-- Step 1 - Uncomment CREATE and Comment out Step 2 and Step 3 and run to create table
-- CREATE TABLE todo (
--     id INTEGER PRIMARY KEY AUTOINCREMENT,
--     title TEXT NOT NULL,
--     description TEXT,
--     is_completed BOOLEAN NOT NULL DEFAULT 0,
--     due_date DATETIME
-- );

-- Step 2 - Uncomment INSERT and Comment out Step 1 and Step 3 and run to insert rows
-- INSERT INTO todo (title, description, is_completed, due_date) VALUES
-- ('Buy groceries', 'Milk, eggs, bread, and coffee', 0, '2025-06-20 18:00:00'),
-- ('Call plumber', 'Fix leaking sink in kitchen', 0, '2025-06-19 10:00:00'),
-- ('Finish report', 'Complete the monthly sales report for manager', 0, '2025-06-21 17:00:00'),
-- ('Workout', '30 minutes cardio and strength training', 1, '2025-06-17 07:00:00'),
-- ('Email Sarah', 'Send project update and next steps', 0, '2025-06-18 15:00:00'),
-- ('Book flight', 'Buy ticket for vacation in July', 1, '2025-06-15 20:00:00'),
-- ('Read book', 'Read 50 pages of "Atomic Habits"', 0, '2025-06-22 21:00:00'),
-- ('Team meeting', 'Weekly sync with dev team', 0, '2025-06-19 09:30:00'),
-- ('Pay electricity bill', 'Due this week', 0, '2025-06-20 23:59:00'),
-- ('Clean garage', 'Organize tools and boxes', 0, '2025-06-23 14:00:00');

-- Step 3 - Uncomment SELECT and Comment out Step 1 and Step 2 and run to see records
SELECT * FROM todo;