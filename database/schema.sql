PRAGMA foreign_keys = ON;
CREATE TABLE accounts (id INTEGER PRIMARY KEY, code TEXT NOT NULL UNIQUE, name TEXT NOT NULL, type TEXT NOT NULL CHECK(type IN ('Asset','Liability','Equity','Revenue','Expense')));
CREATE TABLE invoices (id INTEGER PRIMARY KEY, customer TEXT NOT NULL, number TEXT NOT NULL UNIQUE, issue_date TEXT NOT NULL, due_date TEXT NOT NULL, amount NUMERIC NOT NULL CHECK(amount > 0), status TEXT NOT NULL CHECK(status IN ('Draft','Sent','Paid','Overdue')));
CREATE TABLE journal_entries (id INTEGER PRIMARY KEY, entry_date TEXT NOT NULL, memo TEXT NOT NULL);
CREATE TABLE journal_lines (id INTEGER PRIMARY KEY, journal_entry_id INTEGER NOT NULL REFERENCES journal_entries(id) ON DELETE CASCADE, account_id INTEGER NOT NULL REFERENCES accounts(id), debit NUMERIC NOT NULL DEFAULT 0 CHECK(debit >= 0), credit NUMERIC NOT NULL DEFAULT 0 CHECK(credit >= 0), CHECK((debit = 0 AND credit > 0) OR (credit = 0 AND debit > 0)));
CREATE INDEX ix_journal_entries_date ON journal_entries(entry_date);
CREATE INDEX ix_journal_lines_entry ON journal_lines(journal_entry_id);

