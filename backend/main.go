package main

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"

	"github.com/jackc/pgx/v5"
)

type HealthResponse struct {
	Status   string `json:"status"`
	Database string `json:"database"`
}

func healthHandler(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Content-Type", "application/json")

	connString := os.Getenv("DATABASE_URL")
	if connString == "" {
		connString = "postgres://admin:secret@db:5432/florarium_db"
	}

	conn, err := pgx.Connect(context.Background(), connString)
	if err != nil {
		w.WriteHeader(http.StatusServiceUnavailable)
		json.NewEncoder(w).Encode(HealthResponse{
			Status:   "error",
			Database: "unreachable: " + err.Error(),
		})
		return
	}
	defer conn.Close(context.Background())

	var result int
	err = conn.QueryRow(context.Background(), "SELECT 1").Scan(&result)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		json.NewEncoder(w).Encode(HealthResponse{
			Status:   "error",
			Database: "query failed: " + err.Error(),
		})
		return
	}

	json.NewEncoder(w).Encode(HealthResponse{
		Status:   "ok",
		Database: fmt.Sprintf("SELECT 1 = %d", result),
	})
}

func main() {
	http.HandleFunc("/health", healthHandler)

	fmt.Println("Go-сервер запущен на порту :8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
