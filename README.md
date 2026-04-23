# Atma (수집형 RPG)

**Project Atma**는 수집형 RPG 장르의 기본적인 콘텐츠를 구현한 중심 프로젝트입니다.

---

## Tech Stack
* **Language & Framework**: ASP.NET Core, Blazor Server
* **Database**: MSSQL (EF Core, Dapper), Redis
* **DevOps**: Docker

---

## Server Architecture
* **Login Server**: JWT와 Dapper를 사용하여 가벼운 인증 및 세션 관리 수행
* **Game Server**: EF Core를 활용한 데이터 모델링 및 게임 비즈니스 로직 처리
* **Admin Tool**: Blazor Server를 활용해 운영 기능 제공
* **Redis Layer**: 세션 관리, 분산 락, 데이터 캐싱을 통한 성능 최적화

---

## Database Schema
데이터 효율성과 확장성을 위해 마스터(Master), 계정(Account), 유저(Person) 영역을 분리하여 설계했습니다.

* **Master DB**: 아이템, 가챠 확률, 업적, 배너 등 게임의 정적 데이터 관리
* **Account DB**: 계정 정보 관리
* **Person DB**: 유저의 게임 플레이 데이터 관리

---

## Info
* **개발 기간**: 2025.11 ~ 개발중
* **개발 인원**: 5명 (클라이언트 1명, 서버 1명, UI 1명, 애니메이션 1명, 일러스트 1명)
* **담당 역할**: 서버 프로그래머