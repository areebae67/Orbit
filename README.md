# Orbit – Curriculum Intelligence and Analysis System

## Overview

Orbit is a curriculum intelligence platform designed to transform unstructured university curriculum documents (PDF booklets, scheme-of-study files, and academic outlines) into structured, normalized, and analyzable datasets.

The system extracts academic information such as courses, credit structures, and program layouts, and converts them into a consistent schema that can be used for cross-university comparison, academic planning, and curriculum analysis.

Unlike traditional document parsers, Orbit is designed as a multi-stage intelligence pipeline that progressively converts raw text into structured academic knowledge. It is built to handle variations in formatting across different universities and attempt structural inference where explicit formatting is missing or inconsistent.

The long-term goal of Orbit is to enable semantic comparison between university curricula (e.g., FAST vs NUST vs AIR vs NUML) by identifying equivalence between courses, mapping skills, and analyzing curriculum depth and progression.

---

## Key Capabilities

### Curriculum Ingestion

* Upload and process university curriculum PDFs
* Extract raw text from structured and unstructured documents
* Handle multiple curriculum formats including booklets and scheme tables

### Text Processing Pipeline

* PDF text extraction and normalization
* Noise removal and formatting cleanup
* Block segmentation of curriculum content

### Structure Inference Engine

* Detect course boundaries from unstructured text
* Identify course metadata such as codes, titles, and credit hours
* Attempt inference of academic hierarchy (program → semester → course)

### Course Normalization

Each course is transformed into a structured entity containing:

* Course title
* Course code
* Credit hours
* Theory and lab breakdown
* Difficulty and classification metadata
* Source format identification
* Internal scoring metrics (depth, breadth, practical balance)

### API-Based Architecture

* RESTful endpoints for curriculum processing
* Debug endpoints for structure validation
* Modular service-based backend design

---

## System Workflow

Orbit follows a multi-stage processing pipeline:

1. PDF Upload via API
2. Raw text extraction from document
3. Text cleaning and normalization
4. Structural block detection
5. Course extraction and parsing
6. Metadata enrichment and classification
7. JSON-based structured output generation

---

## Tech Stack

### Backend

* ASP.NET Core Web API
* C#
* Clean Architecture (Layered Design)

### Processing Layer

* Custom PDF parsing engine
* Rule-based structure inference system
* Curriculum segmentation and normalization services

### Data Representation

* JSON-based intermediate and final models
* Entity-driven curriculum modeling

---

## Project Architecture

Orbit follows a modular layered architecture:

### API Layer (Orbit.Web)

Handles HTTP requests, file uploads, and curriculum processing endpoints.

### Application Layer (Orbit.Application)

Contains business logic, orchestration, and use-case implementations.

### Infrastructure Layer (Orbit.Infrastructure)

Responsible for:

* PDF extraction
* Text processing
* Structure inference engine
* Parsing and transformation logic

### Domain Layer

Defines core entities such as:

* Curriculum
* Course
* University
* Academic structure models

---

## Current Output Example

A successfully processed curriculum produces structured course data such as:

* Course Title: Data Structures
* Course Code: CSDS-224
* Credit Hours: 4
* Theory Hours: 3
* Lab Hours: 1
* Assessment Style: Mixed
* Difficulty Level: Intermediate
* Source Format: SchemeTable

---

## Current Capabilities (MVP Status)

* Working PDF ingestion pipeline
* Structured course extraction
* Format detection (Booklet / SchemeTable)
* Basic metadata generation
* Stable API response system

---

## Known Limitations

Orbit is still under active development and currently has the following limitations:

### Structural Gaps

* Semester-level hierarchy is not fully reconstructed
* Courses are currently extracted as a flat structure

### Semantic Gaps

* Topics, learning outcomes, and skill tags are not yet populated
* Limited semantic understanding of course content

### Comparison Layer

* Cross-university comparison engine is not yet implemented
* No similarity scoring or curriculum alignment system

### AI Layer

* No deep learning or embedding-based semantic modeling is currently active

---

## Roadmap

### Phase 1: Structural Completion

* Implement reliable semester detection
* Improve curriculum hierarchy reconstruction
* Stabilize parsing across different university formats

### Phase 2: Semantic Enrichment

* Extract course topics and learning outcomes
* Generate skill tags per course
* Improve metadata classification using AI-assisted methods

### Phase 3: Comparison Engine

* Enable cross-university curriculum comparison
* Implement course similarity matching
* Detect skill gaps and overlaps between programs

### Phase 4: Intelligence Layer

* Build embedding-based curriculum understanding
* Add semantic search and recommendation system
* Generate curriculum insights and analytics

### Phase 5: Product Layer

* Build visualization dashboard
* Add UI for curriculum exploration
* Enable academic planning tools

---

## Purpose

The purpose of Orbit is to build a structured academic intelligence system that enables:

* Standardization of university curricula
* Cross-university comparison of academic programs
* Identification of skill coverage gaps
* Better academic planning and curriculum analysis

---

## Status

Orbit is currently in active development. The ingestion pipeline is functional, and the system is transitioning from structural extraction toward semantic curriculum intelligence.


